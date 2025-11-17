using System;
using System.Collections.Generic;

namespace HarborMaster.Models
{
    /// <summary>
    /// Comprehensive statistics model for HarborMaster dashboard
    /// Contains aggregated data about harbor operations, ships, berths, and financials
    /// </summary>
    public class HarborStatistics
    {
        // ===== GENERAL STATISTICS =====
        public int TotalShips { get; set; }
        public int TotalBerths { get; set; }
        public int AvailableBerths { get; set; }
        public int OccupiedBerths { get; set; }
        public double OccupancyRate { get; set; } // Percentage (0-100)
        public decimal TotalRevenue { get; set; }
        public int TotalAssignments { get; set; }

        // ===== CURRENT STATUS =====
        public int ShipsCurrentlyDocked { get; set; }
        public int ShipsScheduled { get; set; }
        public int ShipsDeparted { get; set; }

        // ===== REQUEST STATISTICS =====
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalRequests { get; set; }
        public double ApprovalRate { get; set; } // Percentage (0-100)

        // ===== SHIP TYPE DISTRIBUTION =====
        public Dictionary<string, int> ShipsByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> RevenueByShipType { get; set; } = new Dictionary<string, decimal>();

        // ===== BERTH STATISTICS =====
        public Dictionary<int, int> AssignmentsByBerth { get; set; } = new Dictionary<int, int>();
        public List<BerthUtilization> BerthUtilizations { get; set; } = new List<BerthUtilization>();

        // ===== TIME-BASED STATISTICS =====
        public int ArrivalsToday { get; set; }
        public int DeparturesToday { get; set; }
        public int ArrivalsThisWeek { get; set; }
        public int DeparturesThisWeek { get; set; }
        public int ArrivalsThisMonth { get; set; }
        public int DeparturesThisMonth { get; set; }

        // ===== FINANCIAL STATISTICS =====
        public decimal RevenueToday { get; set; }
        public decimal RevenueThisWeek { get; set; }
        public decimal RevenueThisMonth { get; set; }
        public decimal AverageRevenuePerShip { get; set; }

        // ===== OPERATIONAL METRICS =====
        public double AverageDockingDuration { get; set; } // In days
        public double AverageProcessingTime { get; set; } // Average time to process requests (in hours)
        public int OverdueShips { get; set; } // Ships that exceeded their ETD

        // ===== UPCOMING SCHEDULE =====
        public List<BerthAssignment> UpcomingArrivals { get; set; } = new List<BerthAssignment>();
        public List<BerthAssignment> UpcomingDepartures { get; set; } = new List<BerthAssignment>();

        // ===== HELPER METHODS =====

        /// <summary>
        /// Calculate occupancy rate based on occupied vs total berths
        /// </summary>
        public void CalculateOccupancyRate()
        {
            if (TotalBerths > 0)
            {
                OccupancyRate = (double)OccupiedBerths / TotalBerths * 100;
            }
            else
            {
                OccupancyRate = 0;
            }
        }

        /// <summary>
        /// Calculate approval rate based on approved vs total requests
        /// </summary>
        public void CalculateApprovalRate()
        {
            int processedRequests = ApprovedRequests + RejectedRequests;
            if (processedRequests > 0)
            {
                ApprovalRate = (double)ApprovedRequests / processedRequests * 100;
            }
            else
            {
                ApprovalRate = 0;
            }
        }

        /// <summary>
        /// Calculate average revenue per ship
        /// </summary>
        public void CalculateAverageRevenue()
        {
            if (TotalAssignments > 0)
            {
                AverageRevenuePerShip = TotalRevenue / TotalAssignments;
            }
            else
            {
                AverageRevenuePerShip = 0;
            }
        }

        /// <summary>
        /// Get formatted occupancy rate string
        /// </summary>
        public string GetOccupancyRateDisplay()
        {
            return $"{OccupancyRate:F1}% ({OccupiedBerths}/{TotalBerths})";
        }

        /// <summary>
        /// Get formatted approval rate string
        /// </summary>
        public string GetApprovalRateDisplay()
        {
            return $"{ApprovalRate:F1}% ({ApprovedRequests}/{ApprovedRequests + RejectedRequests})";
        }

        /// <summary>
        /// Get the most popular ship type
        /// </summary>
        public string GetMostPopularShipType()
        {
            if (ShipsByType.Count == 0) return "N/A";

            string mostPopular = "";
            int maxCount = 0;

            foreach (var kvp in ShipsByType)
            {
                if (kvp.Value > maxCount)
                {
                    maxCount = kvp.Value;
                    mostPopular = kvp.Key;
                }
            }

            return $"{mostPopular} ({maxCount})";
        }

        /// <summary>
        /// Get the most utilized berth
        /// </summary>
        public string GetMostUtilizedBerth()
        {
            if (AssignmentsByBerth.Count == 0) return "N/A";

            int mostUsedBerthId = 0;
            int maxAssignments = 0;

            foreach (var kvp in AssignmentsByBerth)
            {
                if (kvp.Value > maxAssignments)
                {
                    maxAssignments = kvp.Value;
                    mostUsedBerthId = kvp.Key;
                }
            }

            return $"Berth #{mostUsedBerthId} ({maxAssignments} assignments)";
        }
    }

    /// <summary>
    /// Detailed berth utilization information
    /// </summary>
    public class BerthUtilization
    {
        public int BerthId { get; set; }
        public string BerthName { get; set; } = string.Empty;
        public int TotalAssignments { get; set; }
        public double UtilizationRate { get; set; } // Percentage (0-100)
        public bool IsCurrentlyOccupied { get; set; }
        public DateTime? CurrentOccupiedUntil { get; set; }
        public decimal TotalRevenue { get; set; }

        public string GetUtilizationDisplay()
        {
            return $"{UtilizationRate:F1}% ({TotalAssignments} assignments)";
        }
    }

    /// <summary>
    /// Time-series data point for trend analysis
    /// </summary>
    public class TimeSeriesDataPoint
    {
        public DateTime Date { get; set; }
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
        public decimal Revenue { get; set; }
    }
}
