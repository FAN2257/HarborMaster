using HarborMaster.Models;
using HarborMaster.Services; // <-- Make sure AuthenticationService is in this namespace
using HarborMaster.Views.Interfaces; // <-- Menggunakan interface
using System.Threading.Tasks;

namespace HarborMaster.Presenters
{
    public class LoginPresenter
    {
        // 1. Referensi ke View (melalui interface, bukan Form)
        private readonly ILoginView _view;

        // 2. Referensi ke Service (untuk logika bisnis)
        private readonly AuthenticationService _authService;
        private readonly NotificationService _notificationService;

        public LoginPresenter(ILoginView view)
        {
            _view = view;

            // 3. Inisialisasi service yang dibutuhkan
            _authService = new AuthenticationService();
            _notificationService = new NotificationService();
        }

        public async Task LoginAsync()
        {
            // Set status loading di UI
            _view.IsLoading = true;
            _view.ErrorMessage = string.Empty; 

            try
            {
                // 1. Ambil input dari View (melalui interface)
                string email = _view.Email;
                string password = _view.Password;

                // 2. Panggil Service untuk validasi (nullable return)
                User? user = await _authService.ValidateUser(email, password);

                if (user != null)
                {
                    // 3. Sukses! Beri tahu View untuk lanjut
                    _view.HandleLoginSuccess(user);
                    _view.CloseView();
                }
                else
                {
                    // 4. Gagal. Beri tahu View untuk menampilkan error
                    _view.ErrorMessage = "Email atau Password salah.";
                }
            }
            catch (System.Exception ex)
            {
                // Tangani error koneksi database, dll.
                _view.ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                // Selalu matikan status loading
                _view.IsLoading = false;
            }
        }
    }
}