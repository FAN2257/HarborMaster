using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class ResetPasswordWindow : Form, IResetPasswordView
    {
        private readonly ResetPasswordPresenter _presenter;

        // For draggable window
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        // UI Controls
        private Panel panelMain = null!;
        private Panel panelTitleBar = null!;
        private Label lblTitle = null!;
        private Label lblResetCode = null!;
        private TextBox txtResetCode = null!;
        private Label lblNewPassword = null!;
        private TextBox txtNewPassword = null!;
        private Label lblConfirmPassword = null!;
        private TextBox txtConfirmPassword = null!;
        private Button btnReset = null!;
        private Button btnCancel = null!;
        private Label lblError = null!;
        private Button btnClose = null!;
        private Label lblInfo = null!;

        public ResetPasswordWindow()
        {
            InitializeComponent();
            _presenter = new ResetPasswordPresenter(this);
        }

        private void InitializeComponent()
        {
            // Form Settings
            this.Text = "Reset Password - HarborMaster";
            this.Size = new Size(550, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // Title Bar
            panelTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(52, 152, 219)
            };
            panelTitleBar.MouseDown += panelTitleBar_MouseDown;

            // Close Button
            btnClose = new Button
            {
                Text = "?",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 40, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // Title Bar Label
            var lblTitleBar = new Label
            {
                Text = "RESET PASSWORD",
                Location = new Point(15, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White
            };

            panelTitleBar.Controls.AddRange(new Control[] { btnClose, lblTitleBar });

            // Panel Main
            panelMain = new Panel
            {
                Location = new Point(50, 80),
                Size = new Size(450, 470),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Title
            lblTitle = new Label
            {
                Text = "RESET PASSWORD",
                Location = new Point(30, 30),
                Size = new Size(390, 40),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Info Label
            lblInfo = new Label
            {
                Text = "Masukkan kode reset yang telah dikirim ke email Anda",
                Location = new Point(30, 75),
                Size = new Size(390, 35),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.TopCenter
            };

            // Reset Code Label
            lblResetCode = new Label
            {
                Text = "Kode Reset (6 digit):",
                Location = new Point(40, 125),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            // Reset Code TextBox
            txtResetCode = new TextBox
            {
                Location = new Point(40, 155),
                Size = new Size(370, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 6
            };

            // New Password Label
            lblNewPassword = new Label
            {
                Text = "Password Baru:",
                Location = new Point(40, 205),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            // New Password TextBox
            txtNewPassword = new TextBox
            {
                Location = new Point(40, 235),
                Size = new Size(370, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '?'
            };

            // Confirm Password Label
            lblConfirmPassword = new Label
            {
                Text = "Konfirmasi Password:",
                Location = new Point(40, 285),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            // Confirm Password TextBox
            txtConfirmPassword = new TextBox
            {
                Location = new Point(40, 315),
                Size = new Size(370, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '?'
            };

            // Error Label
            lblError = new Label
            {
                Text = "",
                Location = new Point(40, 360),
                Size = new Size(370, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(220, 38, 38),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            // Reset Button
            btnReset = new Button
            {
                Text = "RESET PASSWORD",
                Location = new Point(40, 400),
                Size = new Size(180, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += async (s, e) => await _presenter.ResetPasswordAsync();

            // Cancel Button
            btnCancel = new Button
            {
                Text = "BATAL",
                Location = new Point(230, 400),
                Size = new Size(180, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(148, 163, 184),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            // Add controls to panel
            panelMain.Controls.AddRange(new Control[]
            {
                lblTitle, lblInfo, lblResetCode, txtResetCode,
                lblNewPassword, txtNewPassword,
                lblConfirmPassword, txtConfirmPassword,
                lblError, btnReset, btnCancel
            });

            // Add panels to form
            this.Controls.AddRange(new Control[] { panelTitleBar, panelMain });
        }

        // IResetPasswordView Implementation
        public string ResetCode => txtResetCode.Text.Trim();
        public string NewPassword => txtNewPassword.Text;
        public string ConfirmPassword => txtConfirmPassword.Text;

        public string ErrorMessage
        {
            set
            {
                lblError.Text = value;
                lblError.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsLoading
        {
            set
            {
                btnReset.Enabled = !value;
                txtResetCode.Enabled = !value;
                txtNewPassword.Enabled = !value;
                txtConfirmPassword.Enabled = !value;
                btnReset.Text = value ? "MEMPROSES..." : "RESET PASSWORD";
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        public void ShowSuccessAndClose()
        {
            this.DialogResult = DialogResult.OK;
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
    }
}
