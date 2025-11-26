using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface
    public partial class RegisterWindow : Form, IRegisterView
    {
        // 2. Pegang referensi Presenter
        private readonly RegisterPresenter _presenter;

        public RegisterWindow()
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!

            // 3. Buat instance Presenter
            _presenter = new RegisterPresenter(this);
        }

        // --- Implementasi Interface IRegisterView ---
        public string Username => txtUsername.Text;
        public string Password => txtPassword.Text;
        public string ConfirmPassword => txtConfirmPassword.Text;
        public string FullName => txtFullName.Text;

        public string ErrorMessage
        {
            set { lblError.Text = value; }
        }

        public bool IsLoading
        {
            set
            {
                btnRegister.Enabled = !value;
                btnRegister.Text = value ? "LOADING..." : "DAFTAR";
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

        // --- Event Handler ---
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            // 5. Hubungkan ke Presenter
            await _presenter.RegisterAsync();
        }
    }
}