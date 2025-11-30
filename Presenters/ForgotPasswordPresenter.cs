using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class ForgotPasswordPresenter
    {
        private readonly IForgotPasswordView _view;
        private readonly AuthenticationService _authService;
        private readonly NotificationService _notificationService;

        public ForgotPasswordPresenter(IForgotPasswordView view)
        {
            _view = view;
            _authService = new AuthenticationService();
            _notificationService = new NotificationService();
        }

        public async Task RequestResetCodeAsync()
        {
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty;
            _view.SuccessMessage = string.Empty;

            try
            {
                string email = _view.Email;

                // Validasi input
                if (string.IsNullOrWhiteSpace(email))
                {
                    _view.ErrorMessage = "Email tidak boleh kosong.";
                    return;
                }

                // Request reset code
                string errorMessage = await _authService.RequestPasswordReset(email);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // Sukses - email telah dikirim
                    _view.SuccessMessage = "Kode reset password telah dikirim ke email Anda. Silakan cek inbox Anda.";
                    
                    _notificationService.ShowMessage(
                        "Email Terkirim!\n\n" +
                        "Kode reset password telah dikirim ke email Anda.\n" +
                        "Silakan cek inbox (atau folder spam) Anda.\n\n" +
                        "Kode berlaku selama 30 menit."
                    );
                }
                else
                {
                    // Error message dari service
                    _view.ErrorMessage = errorMessage;
                }
            }
            catch (System.Exception ex)
            {
                string fullError = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullError += $"\nDetail: {ex.InnerException.Message}";
                }

                _notificationService.ShowError(fullError);
                _view.ErrorMessage = "Gagal memproses permintaan. Cek notifikasi untuk detail.";
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
