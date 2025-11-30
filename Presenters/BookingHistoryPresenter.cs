using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for Booking History view (ShipOwner)
    /// Handles loading and filtering docking requests for the current user
    /// </summary>
    public class BookingHistoryPresenter
    {
        private readonly IBookingHistoryView _view;
        private readonly DockingRequestRepository _requestRepo;
        private readonly User _currentUser;
        private List<DockingRequest> _allBookings; // Cache for filtering

        public BookingHistoryPresenter(IBookingHistoryView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestRepo = new DockingRequestRepository();
            _allBookings = new List<DockingRequest>();
        }

        /// <summary>
        /// Load all booking history for current user
        /// </summary>
        public async Task LoadBookingsAsync()
        {
            try
            {
                _view.IsLoading = true;

                // Get all docking requests
                var allRequests = await _requestRepo.GetAllAsync();

                // Get only requests for ships owned by current user
                var shipRepo = new ShipRepository();
                var userShips = await shipRepo.GetAllAsync();
                var userShipIds = userShips
                    .Where(s => s.OwnerId == _currentUser.Id)
                    .Select(s => s.Id)
                    .ToList();

                // Filter requests for user's ships and cache
                _allBookings = allRequests
                    .Where(r => userShipIds.Contains(r.ShipId))
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                // Apply search and filter
                ApplySearchAndFilter();

                if (_allBookings.Count == 0)
                {
                    _view.ShowMessage("Anda belum memiliki riwayat pemesanan docking.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading bookings: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Apply search and filter to bookings list
        /// </summary>
        public void ApplySearchAndFilter()
        {
            try
            {
                var searchTerm = _view.SearchTerm?.Trim().ToLower() ?? "";
                var selectedStatus = _view.SelectedStatus;

                // Start with all bookings
                var filteredBookings = new List<DockingRequest>(_allBookings);

                // Apply search term (request ID or ship name)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    // Note: We need to join with Ship to search by ship name
                    // For now, we'll just search by request ID
                    filteredBookings = filteredBookings
                        .Where(r => r.Id.ToString().Contains(searchTerm))
                        .ToList();
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(selectedStatus) && selectedStatus != "All Status")
                {
                    filteredBookings = filteredBookings
                        .Where(r => r.Status == selectedStatus)
                        .ToList();
                }

                // Update view with filtered results
                _view.SetBookingsDataSource(filteredBookings);
                _view.UpdateResultCount(filteredBookings.Count, _allBookings.Count);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error filtering bookings: {ex.Message}");
            }
        }
    }
}
