using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface ILoginView
    public partial class LoginWindow : Form, ILoginView
    {
        // 2. Pegang referensi ke Presenter
        private readonly LoginPresenter _presenter;

        public LoginWindow()
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!

            // 3. Buat instance Presenter
            _presenter = new LoginPresenter(this);
        }

        // --- Implementasi Interface ILoginView ---

        public string Username => txtUsername?.Text ?? "";
        public string Password => txtPassword?.Text ?? "";
        public User? LoggedInUser { get; private set; }

        public string ErrorMessage
        {
            set { if (lblError != null) lblError.Text = value; }
        }

        public bool IsLoading
        {
            set
            {
                // Nonaktifkan tombol saat loading
                if (btnLogin != null) btnLogin.Enabled = !value;
                if (txtUsername != null) txtUsername.Enabled = !value;
                if (txtPassword != null) txtPassword.Enabled = !value;
                if (btnLogin != null) btnLogin.Text = value ? "LOADING..." : "LOGIN";
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        public void ShowViewAsDialog()
        {
            this.ShowDialog();
        }

        public void HandleLoginSuccess(User user)
        {
            // Simpan user dan tutup form dengan status OK
            this.LoggedInUser = user;
            this.DialogResult = DialogResult.OK;
        }

        // --- Event Handler ---

        // 5. Hubungkan event tombol Login ke Presenter
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await _presenter.LoginAsync();
        }

        // 6. Event handler baru untuk tombol Register
        private void btnGoToRegister_Click(object sender, EventArgs e)
        {
            // Buat dan tampilkan RegisterWindow
            RegisterWindow regWindow = new RegisterWindow();
            regWindow.ShowViewAsDialog(); // Tampilkan sebagai dialog
        }
    }
}