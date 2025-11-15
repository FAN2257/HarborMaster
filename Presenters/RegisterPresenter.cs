using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class RegisterPresenter
    {
        private readonly IRegisterView _view;
        private readonly AuthenticationService _authService;
        private readonly NotificationService _notificationService; // Untuk pesan sukses

        public RegisterPresenter(IRegisterView view)
        {
            _view = view;
            _authService = new AuthenticationService();
            _notificationService = new NotificationService();
        }

        public async Task RegisterAsync()
        {
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty;

            try
            {
                // 1. Ambil input dari View
                string username = _view.Username;
                string password = _view.Password;
                string confirm = _view.ConfirmPassword;
                string fullName = _view.FullName;

                // 2. Validasi Sederhana di Presenter
                if (password != confirm)
                {
                    _view.ErrorMessage = "Password dan Konfirmasi Password tidak cocok.";
                    return;
                }

                // 3. Panggil Service untuk mendaftar
                string errorMessage = await _authService.RegisterUserAsync(username, password, fullName);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // 4. Sukses
                    _notificationService.ShowMessage("Registrasi berhasil! Silakan login.");
                    _view.CloseView();
                }
                else
                {
                    // 5. Gagal (misal: username sudah ada)
                    _view.ErrorMessage = errorMessage;
                }
            }
            catch (System.Exception ex)
            {
                // Mengambil pesan error penuh (termasuk Supabase/Postgrest internal details)
                string fullError = $"Error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    fullError += $"\nDetail: {ex.InnerException.Message}";
                }

                // Tampilkan pesan error lengkap di notifikasi
                _notificationService.ShowError(fullError);

                // Tampilkan pesan sederhana di form
                _view.ErrorMessage = "Gagal. Cek notifikasi untuk detail kegagalan Supabase.";
            }
            finally
            {
                _view.IsLoading = false;
            }
        }
    }
}