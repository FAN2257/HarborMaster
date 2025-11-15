using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing; // Diperlukan untuk Point, Size, dan Color
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface ILoginView
    public partial class LoginWindow : Form, ILoginView
    {
        // 2. Pegang referensi ke Presenter
        private readonly LoginPresenter _presenter;

        // --- Variabel Kontrol UI Manual ---
        private RoundedPanel? panelLogin;
        private Label? lblTitle;
        private Label? lblUser;
        private TextBox? txtUsername;
        private Label? lblPass;
        private TextBox? txtPassword;
        private Button? btnLogin;
        private Button? btnGoToRegister; // Tombol baru
        private Label? lblError;
        // ----------------------------------

        public LoginWindow()
        {
            InitializeComponent(); // Memuat Desainer (kosong)

            // 3. Buat instance Presenter
            _presenter = new LoginPresenter(this);

            // 4. Panggil metode untuk membangun UI dengan kode
            InitializeManualUI();
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


        // --- KODE DESAIN MANUAL ---

        private void InitializeManualUI()
        {
            // Atur Form Utama (Window)
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(89, 171, 227); // Biru gradient-like
            this.ClientSize = new Size(900, 600);

            // Buat Panel Putih dengan Shadow Effect
            panelLogin = new RoundedPanel();
            panelLogin.BackColor = Color.White;
            panelLogin.Location = new Point(300, 125); // Centered
            panelLogin.Size = new Size(350, 420);
            panelLogin.CornerRadius = 15;

            // Buat Judul "HarborMaster"
            lblTitle = new Label();
            lblTitle.Text = "HarborMaster";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94); // Dark blue-grey
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(70, 40);

            // Buat Subjudul
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "Masukkan kredensial Anda untuk masuk";
            lblSubtitle.Font = new Font("Segoe UI", 9);
            lblSubtitle.ForeColor = Color.Gray;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(70, 75);

            // Buat Label "Username"
            lblUser = new Label();
            lblUser.Text = "Username";
            lblUser.Location = new Point(40, 120);
            lblUser.ForeColor = Color.Gray;
            lblUser.Font = new Font("Segoe UI", 9);

            // Buat TextBox Username
            txtUsername = new TextBox();
            txtUsername.Location = new Point(40, 145);
            txtUsername.Size = new Size(270, 30);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // Buat Label "Password"
            lblPass = new Label();
            lblPass.Text = "Password";
            lblPass.Location = new Point(40, 190);
            lblPass.ForeColor = Color.Gray;
            lblPass.Font = new Font("Segoe UI", 9);

            // Buat TextBox Password
            txtPassword = new TextBox();
            txtPassword.Location = new Point(40, 215);
            txtPassword.Size = new Size(270, 30);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            // Buat Tombol Login
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.BackColor = Color.FromArgb(52, 152, 219); // Blue
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.Location = new Point(40, 270);
            btnLogin.Size = new Size(270, 45);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += new EventHandler(btnLogin_Click);

            // Buat Tombol Register (Link-style button)
            btnGoToRegister = new Button();
            btnGoToRegister.Text = "Belum punya akun? Daftar disini";
            btnGoToRegister.FlatStyle = FlatStyle.Flat;
            btnGoToRegister.FlatAppearance.BorderSize = 0;
            btnGoToRegister.BackColor = Color.FromArgb(100, 181, 246); // Light blue
            btnGoToRegister.ForeColor = Color.White;
            btnGoToRegister.Font = new Font("Segoe UI", 10);
            btnGoToRegister.Location = new Point(40, 325);
            btnGoToRegister.Size = new Size(270, 40);
            btnGoToRegister.Cursor = Cursors.Hand;
            btnGoToRegister.Click += new EventHandler(btnGoToRegister_Click);

            // Buat Label Error
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(40, 375);
            lblError.AutoSize = true;
            lblError.MaximumSize = new Size(270, 0);
            lblError.Font = new Font("Segoe UI", 8);

            // --- Tambahkan Kontrol ke Panel ---
            panelLogin.Controls.Add(lblTitle);
            panelLogin.Controls.Add(lblSubtitle);
            panelLogin.Controls.Add(lblUser);
            panelLogin.Controls.Add(txtUsername);
            panelLogin.Controls.Add(lblPass);
            panelLogin.Controls.Add(txtPassword);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(btnGoToRegister);
            panelLogin.Controls.Add(lblError);

            // --- Tambahkan Panel ke Form ---
            this.Controls.Add(panelLogin);
        }

        // Custom Panel dengan Rounded Corners
        private class RoundedPanel : Panel
        {
            public int CornerRadius { get; set; } = 10;

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                // Enable anti-aliasing
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Create rounded rectangle path
                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = CornerRadius;

                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                // Set region for rounded corners
                this.Region = new Region(path);
            }
        }
    }
}