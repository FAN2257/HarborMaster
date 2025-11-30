using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class ResetPasswordPresenter
    {
        private readonly IResetPasswordView _view;
        private readonly AuthenticationService _authService;
        private readonly NotificationService _notificationService;

        public ResetPasswordPresenter(IResetPasswordView view)
        {
            _view = view;
            _authService = new AuthenticationService();
            _notificationService = new NotificationService();
        }

        public async Task ResetPasswordAsync()
        {
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty;

            try
            {
                // 1. Ambil input dari View
                string resetCode = _view.ResetCode;
                string newPassword = _view.NewPassword;
                string confirmPassword = _view.ConfirmPassword;

                // 2. Validasi input
                if (string.IsNullOrWhiteSpace(resetCode))
                {
                    _view.ErrorMessage = "Kode reset tidak boleh kosong.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    _view.ErrorMessage = "Password baru tidak boleh kosong.";
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    _view.ErrorMessage = "Password dan Konfirmasi Password tidak cocok.";
                    return;
                }

                if (newPassword.Length < 6)
                {
                    _view.ErrorMessage = "Password minimal 6 karakter.";
                    return;
                }

                // 3. Call service untuk reset password
                string errorMessage = await _authService.ResetPassword(resetCode, newPassword);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // Sukses!
                    _notificationService.ShowMessage(
                        "Password Berhasil Diubah!\n\n" +
                        "Password Anda telah berhasil diubah.\n" +
                        "Silakan login dengan password baru Anda."
                    );
                    _view.ShowSuccessAndClose();
                }
                else
                {
                    // Error dari service
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
                _view.ErrorMessage = "Gagal mengubah password. Cek notifikasi untuk detail.";
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}
