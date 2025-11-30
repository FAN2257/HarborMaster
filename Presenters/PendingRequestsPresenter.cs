using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Linq;
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
        private EmailNotificationService? _emailService;
        private WeatherService? _weatherService;

        public PendingRequestsPresenter(IPendingRequestsView view, User currentUser)
        {
            _view = view;
            _currentUser = currentUser;
            _requestService = new DockingRequestService();

            // Initialize email and weather services (gracefully handle if not configured)
            try
            {
                _emailService = new EmailNotificationService();
            }
            catch
            {
                _emailService = null; // Email service not configured
            }

            try
            {
                _weatherService = new WeatherService();
            }
            catch
            {
                _weatherService = null; // Weather service not configured
            }
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

                // Check weather conditions before approval
                if (_weatherService != null)
                {
                    try
                    {
                        var weather = await _weatherService.GetHarborWeatherAsync();
                        if (weather != null && !weather.IsSafeForDocking())
                        {
                            var weatherWarning = System.Windows.Forms.MessageBox.Show(
                                $"⚠️ WEATHER WARNING\n\n" +
                                $"Current weather conditions are UNSAFE for docking:\n\n" +
                                $"{weather.GetSafetyStatus()}\n\n" +
                                $"Do you still want to APPROVE this request?",
                                "Weather Alert",
                                System.Windows.Forms.MessageBoxButtons.YesNo,
                                System.Windows.Forms.MessageBoxIcon.Warning
                            );

                            if (weatherWarning == System.Windows.Forms.DialogResult.No)
                            {
                                _view.IsLoading = false;
                                return; // Cancel approval
                            }
                        }
                    }
                    catch
                    {
                        // Weather check failed, continue anyway
                    }
                }

                // Allow override for HarborMaster
                bool allowOverride = _currentUser.Role == UserRole.HarborMaster;

                var result = await _requestService.ApproveRequest(requestId, _currentUser.Id, allowOverride);

                if (string.IsNullOrEmpty(result))
                {
                    _view.ShowMessage("Request berhasil diapprove dan dermaga telah dialokasikan!");

                    // Send email notification if service is configured
                    if (_emailService != null)
                    {
                        try
                        {
                            await SendApprovalEmailAsync(requestId);
                        }
                        catch (Exception emailEx)
                        {
                            // Don't fail the approval if email fails
                            System.Diagnostics.Debug.WriteLine($"Email notification failed: {emailEx.Message}");
                        }
                    }

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

                    // Send email notification if service is configured
                    if (_emailService != null)
                    {
                        try
                        {
                            await SendRejectionEmailAsync(requestId, rejectionReason);
                        }
                        catch (Exception emailEx)
                        {
                            // Don't fail the rejection if email fails
                            System.Diagnostics.Debug.WriteLine($"Email notification failed: {emailEx.Message}");
                        }
                    }

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

        /// <summary>
        /// Send email notification after approval
        /// </summary>
        private async Task SendApprovalEmailAsync(int requestId)
        {
            // Fetch all required data for email
            var requestRepo = new DockingRequestRepository();
            var shipRepo = new ShipRepository();
            var berthRepo = new BerthRepository();
            var assignmentRepo = new BerthAssignmentRepository();
            var userRepo = new UserRepository();

            var request = await requestRepo.GetByIdAsync(requestId);
            if (request == null || !request.BerthAssignmentId.HasValue) return;

            var ship = await shipRepo.GetByIdAsync(request.ShipId);
            var assignment = await assignmentRepo.GetByIdAsync(request.BerthAssignmentId.Value);
            if (assignment == null || ship == null || !ship.OwnerId.HasValue) return;

            var berth = await berthRepo.GetByIdAsync(assignment.BerthId);
            var owner = await userRepo.GetByIdAsync(ship.OwnerId.Value);

            if (ship != null && berth != null && owner != null && _emailService != null)
            {
                await _emailService.SendRequestApprovedEmailAsync(owner, request, ship, berth, assignment);
            }
        }

        /// <summary>
        /// Send email notification after rejection
        /// </summary>
        private async Task SendRejectionEmailAsync(int requestId, string rejectionReason)
        {
            // Fetch all required data for email
            var requestRepo = new DockingRequestRepository();
            var shipRepo = new ShipRepository();
            var userRepo = new UserRepository();

            var request = await requestRepo.GetByIdAsync(requestId);
            if (request == null) return;

            var ship = await shipRepo.GetByIdAsync(request.ShipId);
            if (ship == null || !ship.OwnerId.HasValue) return;

            var owner = await userRepo.GetByIdAsync(ship.OwnerId.Value);

            if (ship != null && owner != null && _emailService != null)
            {
                await _emailService.SendRequestRejectedEmailAsync(owner, request, ship, rejectionReason);
            }
        }
    }
}
