using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for Submit Docking Request view (ShipOwner)
    /// </summary>
    public class SubmitDockingRequestPresenter
    {
        private readonly ISubmitDockingRequestView _view;
        private readonly DockingRequestService _requestService;
        private readonly ShipRepository _shipRepo;
        private readonly User _currentUser;

        public SubmitDockingRequestPresenter(ISubmitDockingRequestView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestService = new DockingRequestService();
            _shipRepo = new ShipRepository();
        }

        /// <summary>
        /// Load ships owned by current user for selection
        /// </summary>
        public async Task LoadMyShipsAsync()
        {
            try
            {
                _view.IsLoading = true;

                var allShips = await _shipRepo.GetAllAsync();
                var myShips = allShips.FindAll(s => s.OwnerId == _currentUser.Id);

                _view.SetShipsDataSource(myShips);

                if (myShips.Count == 0)
                {
                    _view.ShowError("Anda belum memiliki kapal terdaftar. Silakan tambahkan kapal terlebih dahulu.");
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
        /// Submit docking request
        /// </summary>
        public async Task SubmitRequestAsync()
        {
            try
            {
                _view.IsLoading = true;

                // Create request object
                var request = new DockingRequest
                {
                    ShipId = _view.SelectedShipId,
                    OwnerId = _currentUser.Id,
                    RequestedETA = _view.RequestedETA,
                    RequestedETD = _view.RequestedETD,
                    CargoType = _view.CargoType,
                    SpecialRequirements = _view.SpecialRequirements,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                // Submit via service
                var result = await _requestService.SubmitRequest(request);

                if (string.IsNullOrEmpty(result))
                {
                    // Success
                    _view.ShowMessage("Docking request berhasil disubmit! Menunggu approval dari operator.");
                    _view.CloseView();
                }
                else
                {
                    // Error
                    _view.ShowError(result);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error submitting request: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
