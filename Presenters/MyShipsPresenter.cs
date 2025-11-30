using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for My Ships view (ShipOwner)
    /// Handles full CRUD operations: Create, Read, Update, Delete
    /// </summary>
    public class MyShipsPresenter
    {
        private readonly IMyShipsView _view;
        private readonly ShipRepository _shipRepo;
        private readonly User _currentUser;
        private List<Ship> _allMyShips; // Cache for filtering

        public MyShipsPresenter(IMyShipsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _shipRepo = new ShipRepository();
            _allMyShips = new List<Ship>();
        }

        /// <summary>
        /// Load ships owned by current user (READ operation)
        /// </summary>
        public async Task LoadMyShipsAsync()
        {
            try
            {
                _view.IsLoading = true;

                // Get all ships
                var allShips = await _shipRepo.GetAllAsync();

                // Filter ships owned by current user and cache
                _allMyShips = allShips.FindAll(s => s.OwnerId == _currentUser.Id);

                // Apply search and filter
                ApplySearchAndFilter();

                if (_allMyShips.Count == 0)
                {
                    _view.ShowMessage("Anda belum memiliki kapal terdaftar.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading ships: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Apply search and filter to ships list
        /// Called when user types in search box or changes filter
        /// </summary>
        public void ApplySearchAndFilter()
        {
            try
            {
                var searchTerm = _view.SearchTerm?.Trim().ToLower() ?? "";
                var selectedType = _view.SelectedShipType;

                // Start with all ships
                var filteredShips = new List<Ship>(_allMyShips);

                // Apply search term (ship name or IMO number)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    filteredShips = filteredShips.FindAll(s =>
                        s.Name.ToLower().Contains(searchTerm) ||
                        s.ImoNumber.ToLower().Contains(searchTerm)
                    );
                }

                // Apply ship type filter
                if (!string.IsNullOrWhiteSpace(selectedType) && selectedType != "All Types")
                {
                    filteredShips = filteredShips.FindAll(s => s.ShipType == selectedType);
                }

                // Update view with filtered results
                _view.SetShipsDataSource(filteredShips);
                _view.UpdateResultCount(filteredShips.Count, _allMyShips.Count);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error filtering ships: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete ship (DELETE operation)
        /// Includes business logic validation before deletion
        /// </summary>
        public async Task DeleteShipAsync(int shipId)
        {
            try
            {
                _view.IsLoading = true;

                // Get ship details for validation
                var ship = await _shipRepo.GetByIdAsync(shipId);

                if (ship == null)
                {
                    _view.ShowError("Kapal tidak ditemukan!");
                    return;
                }

                // Validate ownership
                if (ship.OwnerId != _currentUser.Id)
                {
                    _view.ShowError("Anda tidak memiliki izin untuk menghapus kapal ini!");
                    return;
                }

                // Check for active docking requests
                var requestRepo = new DockingRequestRepository();
                var activeRequests = await requestRepo.GetRequestsByShip(shipId);
                var pendingRequests = activeRequests.FindAll(r => r.Status == "Pending" || r.Status == "Approved");

                if (pendingRequests.Count > 0)
                {
                    var confirmMessage = $"Kapal ini memiliki {pendingRequests.Count} docking request aktif (Pending/Approved).\n\n" +
                                       $"Jika Anda menghapus kapal ini, semua request akan DIBATALKAN.\n\n" +
                                       $"Lanjutkan penghapusan?";

                    var result = System.Windows.Forms.MessageBox.Show(
                        confirmMessage,
                        "?? Warning - Active Requests",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning
                    );

                    if (result == System.Windows.Forms.DialogResult.No)
                    {
                        _view.IsLoading = false;
                        return;
                    }

                    // Cancel all pending/approved requests
                    foreach (var request in pendingRequests)
                    {
                        request.Status = "Cancelled";
                        request.ProcessedAt = DateTime.Now;
                        request.RejectionReason = "Ship deleted by owner";
                        await requestRepo.UpdateAsync(request); // FIXED: Only 1 parameter
                    }
                }

                // Delete the ship - FIXED: Pass ship object, not ID
                await _shipRepo.DeleteAsync(ship);

                _view.ShowMessage($"Kapal '{ship.Name}' berhasil dihapus!\n\n" +
                                $"{pendingRequests.Count} docking request telah dibatalkan.");

                // Reload ship list
                await LoadMyShipsAsync();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Gagal menghapus kapal: {ex.Message}\n\n" +
                              $"Detail: {ex.InnerException?.Message ?? "No details"}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
