using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface ILoginView
    public partial class LoginWindow : Form, ILoginView
    {
        // 2. Pegang referensi ke Presenter
        private readonly LoginPresenter _presenter;

        // For draggable window
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public LoginWindow()
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!

            // 3. Buat instance Presenter
            _presenter = new LoginPresenter(this);

            // 4. Subscribe to resize event to keep form centered
            this.Resize += LoginWindow_Resize;
        }

        // --- Implementasi Interface ILoginView ---

        public string Username => txtEmail?.Text ?? ""; // Email used as username
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
                if (txtEmail != null) txtEmail.Enabled = !value;
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

        // --- Window Control Handlers ---
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐"; // Restore icon
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Text = "□"; // Maximize icon
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Make title bar draggable
        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        // Keep form centered when resizing/maximizing
        private void LoginWindow_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // Center the panel when maximized
                CenterPanel();
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                // Reset to original position when restored
                ResetPanelPosition();
            }
        }

        private void CenterPanel()
        {
            // Center panelLogin in the form
            if (panelLogin != null)
            {
                panelLogin.Left = (this.ClientSize.Width - panelLogin.Width) / 2;
                panelLogin.Top = (this.ClientSize.Height - panelLogin.Height) / 2 + 20; // +20 for title bar
            }
        }

        private void ResetPanelPosition()
        {
            // Reset to original designer position
            if (panelLogin != null)
            {
                panelLogin.Left = 300;  // Original X from Designer
                panelLogin.Top = 165;   // Original Y from Designer
            }
        }

        private void panelLogin_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}