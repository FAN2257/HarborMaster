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

        public MyShipsPresenter(IMyShipsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _shipRepo = new ShipRepository();
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

                // Filter ships owned by current user
                var myShips = allShips.FindAll(s => s.OwnerId == _currentUser.Id);

                _view.SetShipsDataSource(myShips);

                if (myShips.Count == 0)
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
