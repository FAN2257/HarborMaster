using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class ForgotPasswordWindow : Form, IForgotPasswordView
    {
        private readonly ForgotPasswordPresenter _presenter;

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
        private Label lblSubtitle = null!;
        private Label lblEmail = null!;
        private TextBox txtEmail = null!;
        private Button btnRequestCode = null!;
        private Button btnCancel = null!;
        private Label lblError = null!;
        private Label lblSuccess = null!;
        private Button btnClose = null!;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _presenter = new ForgotPasswordPresenter(this);
        }

        private void InitializeComponent()
        {
            // Form Settings
            this.Text = "Lupa Password - HarborMaster";
            this.Size = new Size(550, 500);
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
                Text = "FORGOT PASSWORD",
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
                Size = new Size(450, 360),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Add shadow effect
            panelMain.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            };

            // Title
            lblTitle = new Label
            {
                Text = "LUPA PASSWORD",
                Location = new Point(30, 30),
                Size = new Size(390, 40),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Masukkan email Anda untuk mendapatkan kode reset password",
                Location = new Point(30, 75),
                Size = new Size(390, 50),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.TopCenter
            };

            // Email Label
            lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(40, 140),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85)
            };

            // Email TextBox
            txtEmail = new TextBox
            {
                Location = new Point(40, 170),
                Size = new Size(370, 35),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Error Label
            lblError = new Label
            {
                Text = "",
                Location = new Point(40, 215),
                Size = new Size(370, 30),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(220, 38, 38),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            // Success Label
            lblSuccess = new Label
            {
                Text = "",
                Location = new Point(40, 215),
                Size = new Size(370, 50),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(22, 163, 74),
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            // Request Code Button
            btnRequestCode = new Button
            {
                Text = "KIRIM KODE RESET",
                Location = new Point(40, 270),
                Size = new Size(370, 45),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRequestCode.FlatAppearance.BorderSize = 0;
            btnRequestCode.Click += async (s, e) => await _presenter.RequestResetCodeAsync();

            // Cancel Button
            btnCancel = new Button
            {
                Text = "BATAL",
                Location = new Point(175, 320),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
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
                lblTitle, lblSubtitle, lblEmail, txtEmail,
                lblError, lblSuccess, btnRequestCode, btnCancel
            });

            // Add panels to form
            this.Controls.AddRange(new Control[] { panelTitleBar, panelMain });
        }

        // IForgotPasswordView Implementation
        public string Email => txtEmail.Text.Trim();

        public string ErrorMessage
        {
            set
            {
                lblError.Text = value;
                lblError.Visible = !string.IsNullOrEmpty(value);
                lblSuccess.Visible = false;
            }
        }

        public string SuccessMessage
        {
            set
            {
                lblSuccess.Text = value;
                lblSuccess.Visible = !string.IsNullOrEmpty(value);
                lblError.Visible = false;
            }
        }

        public bool IsLoading
        {
            set
            {
                btnRequestCode.Enabled = !value;
                txtEmail.Enabled = !value;
                btnRequestCode.Text = value ? "MENGIRIM..." : "KIRIM KODE RESET";
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        public void CloseWithSuccess()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
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
