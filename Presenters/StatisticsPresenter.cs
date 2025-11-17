using System;
using System.Threading.Tasks;
using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for HarborMaster Statistics View
    /// Coordinates between StatisticsService and the statistics view
    /// </summary>
    public class StatisticsPresenter
    {
        private readonly IStatisticsView _view;
        private readonly StatisticsService _statisticsService;

        public StatisticsPresenter(IStatisticsView view)
        {
            _view = view;
            _statisticsService = new StatisticsService();
        }

        /// <summary>
        /// Load all statistics data and update the view
        /// </summary>
        public async Task LoadStatisticsAsync()
        {
            try
            {
                _view.IsLoading = true;
                _view.ErrorMessage = string.Empty;

                // Fetch comprehensive statistics
                var stats = await _statisticsService.GetHarborStatisticsAsync();

                // Update general statistics
                _view.SetGeneralStatistics(
                    stats.TotalShips,
                    stats.TotalBerths,
                    stats.GetOccupancyRateDisplay(),
                    stats.TotalRevenue,
                    stats.PendingRequests
                );

                // Update current status
                _view.SetCurrentStatus(
                    stats.ShipsCurrentlyDocked,
                    stats.ShipsScheduled,
                    stats.ShipsDeparted
                );

                // Update request statistics
                _view.SetRequestStatistics(
                    stats.PendingRequests,
                    stats.ApprovedRequests,
                    stats.RejectedRequests,
                    stats.GetApprovalRateDisplay()
                );

                // Update ship type distribution
                _view.SetShipTypeDistribution(stats.ShipsByType);

                // Update revenue by ship type
                _view.SetRevenueByShipType(stats.RevenueByShipType);

                // Update berth utilization
                _view.SetBerthUtilization(stats.BerthUtilizations);

                // Update time-based statistics
                _view.SetTimeBasedStatistics(
                    stats.ArrivalsToday,
                    stats.DeparturesToday,
                    stats.ArrivalsThisWeek,
                    stats.ArrivalsThisMonth,
                    stats.RevenueToday,
                    stats.RevenueThisWeek,
                    stats.RevenueThisMonth
                );

                // Update operational metrics
                _view.SetOperationalMetrics(
                    stats.AverageDockingDuration,
                    stats.AverageProcessingTime,
                    stats.OverdueShips,
                    stats.AverageRevenuePerShip
                );

                // Update upcoming schedule
                _view.SetUpcomingSchedule(
                    stats.UpcomingArrivals,
                    stats.UpcomingDepartures
                );

                // Update top performers
                _view.SetTopPerformers(
                    stats.GetMostPopularShipType(),
                    stats.GetMostUtilizedBerth()
                );

                _view.RefreshCompleted();
            }
            catch (Exception ex)
            {
                _view.ErrorMessage = $"Error loading statistics: {ex.Message}";
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Refresh statistics data
        /// </summary>
        public async Task RefreshStatisticsAsync()
        {
            await LoadStatisticsAsync();
            _view.ShowMessage("Statistics refreshed successfully!");
        }

        /// <summary>
        /// Get revenue trend data for charting
        /// </summary>
        public async Task LoadRevenueTrendAsync(int days = 30)
        {
            try
            {
                var trendData = await _statisticsService.GetRevenueTrendAsync(days);
                // View can handle this data for charting if needed
                // For now, this is available for future enhancement
            }
            catch (Exception ex)
            {
                _view.ErrorMessage = $"Error loading revenue trend: {ex.Message}";
            }
        }

        /// <summary>
        /// Get arrival/departure trend data for charting
        /// </summary>
        public async Task LoadArrivalDepartureTrendAsync(int days = 30)
        {
            try
            {
                var trendData = await _statisticsService.GetArrivalDepartureTrendAsync(days);
                // View can handle this data for charting if needed
                // For now, this is available for future enhancement
            }
            catch (Exception ex)
            {
                _view.ErrorMessage = $"Error loading arrival/departure trend: {ex.Message}";
            }
        }
    }
}
