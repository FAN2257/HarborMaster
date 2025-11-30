using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface
    public partial class RegisterWindow : Form, IRegisterView
    {
        // 2. Pegang referensi Presenter
        private readonly RegisterPresenter _presenter;

        // For draggable window
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public RegisterWindow()
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!

            // 3. Buat instance Presenter
            _presenter = new RegisterPresenter(this);

            // 4. Populate Role ComboBox
            InitializeRoleComboBox();

            // 5. Subscribe to resize event to keep form centered
            this.Resize += RegisterWindow_Resize;
        }

        private void InitializeRoleComboBox()
        {
            cboRole.Items.Clear();
            cboRole.Items.Add(new RoleItem { Display = "Ship Owner (Pemilik Kapal)", Value = UserRole.ShipOwner });
            cboRole.Items.Add(new RoleItem { Display = "Operator Pelabuhan", Value = UserRole.Operator });
            cboRole.Items.Add(new RoleItem { Display = "Harbor Master (Nahkoda)", Value = UserRole.HarborMaster });
            
            cboRole.DisplayMember = "Display";
            cboRole.ValueMember = "Value";
            cboRole.SelectedIndex = 0; // Default: Ship Owner
        }

        // Helper class for ComboBox
        private class RoleItem
        {
            public string Display { get; set; }
            public UserRole Value { get; set; }
        }

        // --- Implementasi Interface IRegisterView ---
        public string Email => txtEmail.Text;
        public string Password => txtPassword.Text;
        public string ConfirmPassword => txtConfirmPassword.Text;
        public string FullName => txtFullName.Text;
        
        public UserRole SelectedRole
        {
            get
            {
                if (cboRole.SelectedItem is RoleItem roleItem)
                {
                    return roleItem.Value;
                }
                return UserRole.ShipOwner; // Default fallback
            }
        }

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
            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                lblError.Text = "Format email tidak valid!";
                return;
            }

            // Validate role selection
            if (cboRole.SelectedItem == null)
            {
                lblError.Text = "Pilih role terlebih dahulu!";
                return;
            }

            // 5. Hubungkan ke Presenter
            await _presenter.RegisterAsync();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
            this.Close();
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
        private void RegisterWindow_Resize(object sender, EventArgs e)
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
            // Center panelRegister in the form
            if (panelRegister != null)
            {
                panelRegister.Left = (this.ClientSize.Width - panelRegister.Width) / 2;
                panelRegister.Top = (this.ClientSize.Height - panelRegister.Height) / 2 + 40; // +40 for title bar
            }
        }

        private void ResetPanelPosition()
        {
            // Reset to original designer position
            if (panelRegister != null)
            {
                panelRegister.Left = 50;  // Original X from Designer
                panelRegister.Top = 90;   // Original Y from Designer
            }
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {
            // Event handler stub
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            // Event handler stub
        }
    }
}