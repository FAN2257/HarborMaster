using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for My Requests view (ShipOwner)
    /// </summary>
    public class MyRequestsPresenter
    {
        private readonly IMyRequestsView _view;
        private readonly DockingRequestService _requestService;
        private readonly User _currentUser;

        public MyRequestsPresenter(IMyRequestsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestService = new DockingRequestService();
        }

        /// <summary>
        /// Load requests submitted by current user
        /// </summary>
        public async Task LoadMyRequestsAsync()
        {
            try
            {
                _view.IsLoading = true;

                var requests = await _requestService.GetRequestsByOwner(_currentUser.Id);

                _view.SetRequestsDataSource(requests);

                if (requests.Count == 0)
                {
                    _view.ShowMessage("Anda belum pernah submit docking request.");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error loading requests: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Cancel a pending request
        /// </summary>
        public async Task CancelRequestAsync(int requestId)
        {
            try
            {
                _view.IsLoading = true;

                var result = await _requestService.CancelRequest(requestId, _currentUser.Id);

                if (string.IsNullOrEmpty(result))
                {
                    _view.ShowMessage("Request berhasil dibatalkan.");
                    await LoadMyRequestsAsync(); // Refresh
                }
                else
                {
                    _view.ShowError(result);
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error canceling request: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
