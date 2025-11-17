using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarborMaster.Models;
using HarborMaster.Repositories;

namespace HarborMaster.Services
{
    /// <summary>
    /// Service for aggregating and calculating harbor statistics
    /// Used primarily by HarborMaster role for dashboard analytics
    /// </summary>
    public class StatisticsService
    {
        private readonly ShipRepository _shipRepo;
        private readonly BerthRepository _berthRepo;
        private readonly BerthAssignmentRepository _assignmentRepo;
        private readonly DockingRequestRepository _requestRepo;
        private readonly InvoiceRepository _invoiceRepo;

        public StatisticsService()
        {
            _shipRepo = new ShipRepository();
            _berthRepo = new BerthRepository();
            _assignmentRepo = new BerthAssignmentRepository();
            _requestRepo = new DockingRequestRepository();
            _invoiceRepo = new InvoiceRepository();
        }

        /// <summary>
        /// Get comprehensive harbor statistics
        /// </summary>
        public async Task<HarborStatistics> GetHarborStatisticsAsync()
        {
            var stats = new HarborStatistics();

            try
            {
                // Fetch all data in parallel for better performance
                var shipsTask = _shipRepo.GetAllAsync();
                var berthsTask = _berthRepo.GetAllAsync();
                var assignmentsTask = _assignmentRepo.GetAllAsync();
                var requestsTask = _requestRepo.GetAllAsync();
                var invoicesTask = _invoiceRepo.GetAllAsync();

                await Task.WhenAll(shipsTask, berthsTask, assignmentsTask, requestsTask, invoicesTask);

                var ships = shipsTask.Result;
                var berths = berthsTask.Result;
                var assignments = assignmentsTask.Result;
                var requests = requestsTask.Result;
                var invoices = invoicesTask.Result;

                // ===== GENERAL STATISTICS =====
                stats.TotalShips = ships.Count;
                stats.TotalBerths = berths.Count;
                stats.AvailableBerths = berths.Count(b => b.Status == "Available");
                stats.OccupiedBerths = berths.Count(b => b.Status == "Occupied");
                stats.TotalAssignments = assignments.Count;
                stats.TotalRevenue = invoices.Sum(i => i.TotalAmount);

                stats.CalculateOccupancyRate();

                // ===== CURRENT STATUS =====
                stats.ShipsCurrentlyDocked = assignments.Count(a =>
                    a.Status == "Arrived" || a.Status == "Docked" || a.Status == "Berlabuh");
                stats.ShipsScheduled = assignments.Count(a =>
                    a.Status == "Scheduled" || a.Status == "Menunggu");
                stats.ShipsDeparted = assignments.Count(a =>
                    a.Status == "Departed" || a.Status == "Berangkat");

                // ===== REQUEST STATISTICS =====
                stats.TotalRequests = requests.Count;
                stats.PendingRequests = requests.Count(r => r.IsPending());
                stats.ApprovedRequests = requests.Count(r => r.IsApproved());
                stats.RejectedRequests = requests.Count(r => r.IsRejected());
                stats.CalculateApprovalRate();

                // ===== SHIP TYPE DISTRIBUTION =====
                stats.ShipsByType = ships
                    .GroupBy(s => s.ShipType ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count());

                // ===== REVENUE BY SHIP TYPE =====
                // Join invoices with assignments and ships to get revenue by ship type
                var assignmentShipMap = new Dictionary<int, Ship>();
                foreach (var assignment in assignments)
                {
                    var ship = ships.FirstOrDefault(s => s.Id == assignment.ShipId);
                    if (ship != null && !assignmentShipMap.ContainsKey(assignment.Id))
                    {
                        assignmentShipMap[assignment.Id] = ship;
                    }
                }

                stats.RevenueByShipType = invoices
                    .Where(inv => assignmentShipMap.ContainsKey(inv.BerthAssignmentId))
                    .GroupBy(inv => assignmentShipMap[inv.BerthAssignmentId].ShipType ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Sum(inv => inv.TotalAmount));

                // ===== BERTH STATISTICS =====
                stats.AssignmentsByBerth = assignments
                    .GroupBy(a => a.BerthId)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Calculate detailed berth utilizations
                foreach (var berth in berths)
                {
                    var berthAssignments = assignments.Where(a => a.BerthId == berth.Id).ToList();
                    var berthInvoices = invoices.Where(inv =>
                        berthAssignments.Any(ba => ba.Id == inv.BerthAssignmentId)).ToList();

                    var currentAssignment = berthAssignments.FirstOrDefault(a =>
                        a.Status == "Arrived" || a.Status == "Docked" || a.Status == "Berlabuh");

                    stats.BerthUtilizations.Add(new BerthUtilization
                    {
                        BerthId = berth.Id,
                        BerthName = $"Berth #{berth.Id}",
                        TotalAssignments = berthAssignments.Count,
                        UtilizationRate = CalculateBerthUtilizationRate(berthAssignments),
                        IsCurrentlyOccupied = berth.Status == "Occupied",
                        CurrentOccupiedUntil = currentAssignment?.ETD,
                        TotalRevenue = berthInvoices.Sum(i => i.TotalAmount)
                    });
                }

                // ===== TIME-BASED STATISTICS =====
                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                stats.ArrivalsToday = assignments.Count(a =>
                    a.ActualArrivalTime.HasValue && a.ActualArrivalTime.Value.Date == today);
                stats.DeparturesToday = assignments.Count(a =>
                    a.ActualDepartureTime.HasValue && a.ActualDepartureTime.Value.Date == today);

                stats.ArrivalsThisWeek = assignments.Count(a =>
                    a.ActualArrivalTime.HasValue && a.ActualArrivalTime.Value >= startOfWeek);
                stats.DeparturesThisWeek = assignments.Count(a =>
                    a.ActualDepartureTime.HasValue && a.ActualDepartureTime.Value >= startOfWeek);

                stats.ArrivalsThisMonth = assignments.Count(a =>
                    a.ActualArrivalTime.HasValue && a.ActualArrivalTime.Value >= startOfMonth);
                stats.DeparturesThisMonth = assignments.Count(a =>
                    a.ActualDepartureTime.HasValue && a.ActualDepartureTime.Value >= startOfMonth);

                // ===== FINANCIAL STATISTICS =====
                stats.RevenueToday = invoices
                    .Where(i => i.IssuedDate.Date == today)
                    .Sum(i => i.TotalAmount);

                stats.RevenueThisWeek = invoices
                    .Where(i => i.IssuedDate >= startOfWeek)
                    .Sum(i => i.TotalAmount);

                stats.RevenueThisMonth = invoices
                    .Where(i => i.IssuedDate >= startOfMonth)
                    .Sum(i => i.TotalAmount);

                stats.CalculateAverageRevenue();

                // ===== OPERATIONAL METRICS =====
                var completedAssignments = assignments.Where(a =>
                    a.ActualArrivalTime.HasValue && a.ActualDepartureTime.HasValue).ToList();

                if (completedAssignments.Any())
                {
                    var durations = completedAssignments
                        .Select(a => (a.ActualDepartureTime!.Value - a.ActualArrivalTime!.Value).TotalDays);
                    stats.AverageDockingDuration = durations.Average();
                }

                var processedRequests = requests.Where(r => r.ProcessedAt.HasValue).ToList();
                if (processedRequests.Any())
                {
                    var processingTimes = processedRequests
                        .Select(r => (r.ProcessedAt!.Value - r.CreatedAt).TotalHours);
                    stats.AverageProcessingTime = processingTimes.Average();
                }

                var now = DateTime.Now;
                stats.OverdueShips = assignments.Count(a =>
                    (a.Status == "Arrived" || a.Status == "Docked" || a.Status == "Berlabuh")
                    && a.ETD < now);

                // ===== UPCOMING SCHEDULE =====
                stats.UpcomingArrivals = assignments
                    .Where(a => a.Status == "Scheduled" && a.ETA >= now && a.ETA <= now.AddDays(7))
                    .OrderBy(a => a.ETA)
                    .Take(10)
                    .ToList();

                stats.UpcomingDepartures = assignments
                    .Where(a => (a.Status == "Arrived" || a.Status == "Docked" || a.Status == "Berlabuh")
                        && a.ETD >= now && a.ETD <= now.AddDays(7))
                    .OrderBy(a => a.ETD)
                    .Take(10)
                    .ToList();

                return stats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating statistics: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Calculate berth utilization rate based on assignments
        /// Simple calculation: (total assignment days / total days in period) * 100
        /// </summary>
        private double CalculateBerthUtilizationRate(List<BerthAssignment> assignments)
        {
            if (assignments.Count == 0) return 0;

            // Calculate based on last 30 days
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var relevantAssignments = assignments.Where(a => a.ETA >= thirtyDaysAgo).ToList();

            if (relevantAssignments.Count == 0) return 0;

            double totalOccupiedDays = 0;
            foreach (var assignment in relevantAssignments)
            {
                var start = assignment.ActualArrivalTime ?? assignment.ETA;
                var end = assignment.ActualDepartureTime ?? assignment.ETD;

                if (end > start)
                {
                    totalOccupiedDays += (end - start).TotalDays;
                }
            }

            // Utilization = (occupied days / 30 days) * 100
            double utilizationRate = (totalOccupiedDays / 30.0) * 100;

            // Cap at 100%
            return Math.Min(utilizationRate, 100);
        }

        /// <summary>
        /// Get revenue trend data for the last N days
        /// </summary>
        public async Task<List<TimeSeriesDataPoint>> GetRevenueTrendAsync(int days = 30)
        {
            var invoices = await _invoiceRepo.GetAllAsync();
            var startDate = DateTime.Today.AddDays(-days);

            var trendData = new List<TimeSeriesDataPoint>();

            for (int i = days; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var dayRevenue = invoices
                    .Where(inv => inv.IssuedDate.Date == date)
                    .Sum(inv => inv.TotalAmount);

                trendData.Add(new TimeSeriesDataPoint
                {
                    Date = date,
                    Label = date.ToString("dd/MM"),
                    Revenue = dayRevenue,
                    Value = (int)dayRevenue
                });
            }

            return trendData;
        }

        /// <summary>
        /// Get arrival/departure trend data for the last N days
        /// </summary>
        public async Task<List<TimeSeriesDataPoint>> GetArrivalDepartureTrendAsync(int days = 30)
        {
            var assignments = await _assignmentRepo.GetAllAsync();
            var startDate = DateTime.Today.AddDays(-days);

            var trendData = new List<TimeSeriesDataPoint>();

            for (int i = days; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var arrivals = assignments
                    .Count(a => a.ActualArrivalTime.HasValue && a.ActualArrivalTime.Value.Date == date);

                trendData.Add(new TimeSeriesDataPoint
                {
                    Date = date,
                    Label = date.ToString("dd/MM"),
                    Value = arrivals
                });
            }

            return trendData;
        }
    }
}
