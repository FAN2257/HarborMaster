using System.Collections.Generic;
using HarborMaster.Models;

namespace HarborMaster.Views.Interfaces
{
    /// <summary>
    /// Interface for HarborMaster Statistics View
    /// Displays comprehensive analytics and metrics about harbor operations
    /// </summary>
    public interface IStatisticsView
    {
        // ===== LOADING STATE =====
        bool IsLoading { get; set; }

        // ===== GENERAL STATISTICS =====
        void SetGeneralStatistics(
            int totalShips,
            int totalBerths,
            string occupancyRate,
            decimal totalRevenue,
            int pendingRequests);

        // ===== CURRENT STATUS =====
        void SetCurrentStatus(
            int shipsCurrentlyDocked,
            int shipsScheduled,
            int shipsDeparted);

        // ===== REQUEST STATISTICS =====
        void SetRequestStatistics(
            int pendingRequests,
            int approvedRequests,
            int rejectedRequests,
            string approvalRate);

        // ===== SHIP TYPE DISTRIBUTION =====
        void SetShipTypeDistribution(Dictionary<string, int> shipsByType);

        // ===== REVENUE BY SHIP TYPE =====
        void SetRevenueByShipType(Dictionary<string, decimal> revenueByType);

        // ===== BERTH UTILIZATION =====
        void SetBerthUtilization(List<BerthUtilization> berthUtilizations);

        // ===== TIME-BASED STATISTICS =====
        void SetTimeBasedStatistics(
            int arrivalsToday,
            int departuresToday,
            int arrivalsThisWeek,
            int arrivalsThisMonth,
            decimal revenueToday,
            decimal revenueThisWeek,
            decimal revenueThisMonth);

        // ===== OPERATIONAL METRICS =====
        void SetOperationalMetrics(
            double averageDockingDuration,
            double averageProcessingTime,
            int overdueShips,
            decimal averageRevenuePerShip);

        // ===== UPCOMING SCHEDULE =====
        void SetUpcomingSchedule(
            List<BerthAssignment> upcomingArrivals,
            List<BerthAssignment> upcomingDepartures);

        // ===== TOP PERFORMERS =====
        void SetTopPerformers(
            string mostPopularShipType,
            string mostUtilizedBerth);

        // ===== ERROR HANDLING =====
        string ErrorMessage { get; set; }
        void ShowMessage(string message);

        // ===== REFRESH ACTION =====
        void RefreshCompleted();
    }
}
