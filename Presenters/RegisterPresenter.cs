using HarborMaster.Models;
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
                string email = _view.Email;
                string password = _view.Password;
                string confirm = _view.ConfirmPassword;
                string fullName = _view.FullName;
                UserRole selectedRole = _view.SelectedRole; // Get selected role

                // 2. Validasi Sederhana di Presenter
                if (string.IsNullOrWhiteSpace(email))
                {
                    _view.ErrorMessage = "Email tidak boleh kosong.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    _view.ErrorMessage = "Nama lengkap tidak boleh kosong.";
                    return;
                }

                if (password != confirm)
                {
                    _view.ErrorMessage = "Password dan Konfirmasi Password tidak cocok.";
                    return;
                }

                if (password.Length < 6)
                {
                    _view.ErrorMessage = "Password minimal 6 karakter.";
                    return;
                }

                // 3. Panggil Service untuk mendaftar dengan role
                string errorMessage = await _authService.RegisterUserAsync(email, password, fullName, selectedRole);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    // 4. Sukses
                    _notificationService.ShowMessage($"Registrasi berhasil sebagai {GetRoleName(selectedRole)}! Silakan login dengan email Anda.");
                    _view.CloseView();
                }
                else
                {
                    // 5. Gagal (misal: email sudah ada)
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

        private string GetRoleName(UserRole role)
        {
            return role switch
            {
                UserRole.ShipOwner => "Ship Owner",
                UserRole.Operator => "Operator",
                UserRole.HarborMaster => "Harbor Master",
                _ => "Unknown"
            };
        }
    }
}