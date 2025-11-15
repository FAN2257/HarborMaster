using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface
    public partial class RegisterWindow : Form, IRegisterView
    {
        // 2. Pegang referensi Presenter
        private readonly RegisterPresenter _presenter;

        // --- Kontrol UI Manual ---
        private Panel panelRegister;
        private Label lblTitle;
        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Label lblError;
        // -------------------------

        public RegisterWindow()
        {
            InitializeComponent();

            // 3. Buat instance Presenter
            _presenter = new RegisterPresenter(this);

            // 4. Panggil metode UI Manual
            InitializeManualUI();
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

        // --- KODE DESAIN MANUAL ---
        private void InitializeManualUI()
        {
            // Atur Form Utama (Window)
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(44, 62, 80); // Latar abu-abu gelap
            this.ClientSize = new Size(400, 600); // Sedikit lebih tinggi

            // Buat Panel Putih
            panelRegister = new Panel();
            panelRegister.BackColor = Color.White;
            panelRegister.Location = new Point(50, 50);
            panelRegister.Size = new Size(300, 500);

            // Buat Judul "Daftar Akun Baru"
            lblTitle = new Label();
            lblTitle.Text = "Daftar Akun Baru";
            lblTitle.Font = new Font("Arial", 18, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(40, 30);

            // (Helper untuk membuat label + textbox)
            int currentY = 90;
            txtFullName = CreateLabeledTextBox("Nama Lengkap", currentY, panelRegister);
            currentY += 70;
            txtUsername = CreateLabeledTextBox("Username", currentY, panelRegister);
            currentY += 70;
            txtPassword = CreateLabeledTextBox("Password", currentY, panelRegister, true);
            currentY += 70;
            txtConfirmPassword = CreateLabeledTextBox("Konfirmasi Password", currentY, panelRegister, true);
            currentY += 80;

            // Buat Tombol Daftar
            btnRegister = new Button();
            btnRegister.Text = "DAFTAR";
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.BackColor = Color.FromArgb(46, 204, 113); // Hijau
            btnRegister.ForeColor = Color.White;
            btnRegister.Font = new Font("Arial", 12, FontStyle.Bold);
            btnRegister.Location = new Point(40, currentY);
            btnRegister.Size = new Size(220, 45);
            btnRegister.Click += new EventHandler(btnRegister_Click);

            // Buat Label Error
            lblError = new Label();
            lblError.Text = "";
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(40, currentY + 55);
            lblError.AutoSize = false;
            lblError.Width = 220;
            lblError.Height = 40;

            // Tambahkan Kontrol ke Panel
            panelRegister.Controls.Add(lblTitle);
            panelRegister.Controls.Add(btnRegister);
            panelRegister.Controls.Add(lblError);

            // Tambahkan Panel ke Form
            this.Controls.Add(panelRegister);
        }

        // Fungsi helper untuk membuat UI lebih cepat
        private TextBox CreateLabeledTextBox(string labelText, int yPos, Control parent, bool isPassword = false)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Location = new Point(40, yPos);
            lbl.ForeColor = Color.Gray;
            parent.Controls.Add(lbl);

            TextBox txt = new TextBox();
            txt.Location = new Point(40, yPos + 25);
            txt.Size = new Size(220, 30);
            txt.Font = new Font("Arial", 12);
            if (isPassword)
                txt.UseSystemPasswordChar = true;
            parent.Controls.Add(txt);

            return txt;
        }
    }
}