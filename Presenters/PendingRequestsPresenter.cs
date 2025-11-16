using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    /// <summary>
    /// Presenter for Pending Requests view (Operator/HarborMaster)
    /// </summary>
    public class PendingRequestsPresenter
    {
        private readonly IPendingRequestsView _view;
        private readonly DockingRequestService _requestService;
        private readonly User _currentUser;

        public PendingRequestsPresenter(IPendingRequestsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestService = new DockingRequestService();
        }

        /// <summary>
        /// Load all pending requests
        /// </summary>
        public async Task LoadPendingRequestsAsync()
        {
            try
            {
                _view.IsLoading = true;

                var requests = await _requestService.GetPendingRequests();

                _view.SetRequestsDataSource(requests);

                if (requests.Count == 0)
                {
                    _view.ShowMessage("Tidak ada pending request saat ini.");
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
        /// Approve a docking request
        /// </summary>
        public async Task ApproveRequestAsync(int requestId)
        {
            try
            {
                _view.IsLoading = true;

                // Check if user has permission (Operator or HarborMaster)
                if (_currentUser.Role != UserRole.Operator && _currentUser.Role != UserRole.HarborMaster)
                {
                    _view.ShowError("Anda tidak memiliki izin untuk approve request.");
                    return;
                }

                // Allow override for HarborMaster
                bool allowOverride = _currentUser.Role == UserRole.HarborMaster;

                var result = await _requestService.ApproveRequest(requestId, _currentUser.Id, allowOverride);

                if (string.IsNullOrEmpty(result))
                {
                    _view.ShowMessage("Request berhasil diapprove dan dermaga telah dialokasikan!");
                    _view.RefreshData(); // Refresh the list
                }
                else
                {
                    _view.ShowError($"Gagal approve request: {result}");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error approving request: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Reject a docking request
        /// </summary>
        public async Task RejectRequestAsync(int requestId, string rejectionReason)
        {
            try
            {
                _view.IsLoading = true;

                // Check if user has permission
                if (_currentUser.Role != UserRole.Operator && _currentUser.Role != UserRole.HarborMaster)
                {
                    _view.ShowError("Anda tidak memiliki izin untuk reject request.");
                    return;
                }

                var result = await _requestService.RejectRequest(requestId, _currentUser.Id, rejectionReason);

                if (string.IsNullOrEmpty(result))
                {
                    _view.ShowMessage("Request berhasil ditolak.");
                    _view.RefreshData(); // Refresh the list
                }
                else
                {
                    _view.ShowError($"Gagal reject request: {result}");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError($"Error rejecting request: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
