using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    // 1. Implementasikan interface IMainView
    public partial class MainWindow : Form, IMainView
    {
        // 2. Pegang referensi ke Presenter dan User
        private readonly MainPresenter _presenter;
        private readonly User _currentUser;

        // --- Kontrol UI Manual ---
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnAddShip;
        private Button btnRefreshData;
        private Button btnBack;

        // Kartu Metrik Dashboard
        private Panel cardTotalKapal;
        private Label lblCardTotalKapalTitle;
        private Label lblCardTotalKapalValue;

        private Panel cardSedangBerlabuh;
        private Label lblCardSedangBerlabuhTitle;
        private Label lblCardSedangBerlabuhValue;

        private Panel cardKapalMenunggu;
        private Label lblCardKapalMenungguTitle;
        private Label lblCardKapalMenungguValue;

        // DataGridView untuk tabel
        private DataGridView dgvSchedule;

        // Untuk alokasi (disimpan untuk interface compatibility)
        private ComboBox comboShips;
        private DateTimePicker dtpETA;
        private DateTimePicker dtpETD;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // 3. Buat instance Presenter
            _presenter = new MainPresenter(this);

            // 4. Panggil metode untuk membangun UI
            InitializeManualUI();

            // 5. Hubungkan event Form_Load untuk memuat data
            this.Load += MainWindow_Load;
        }

        // --- Implementasi Interface IMainView ---

        // 'get' properti (Input dari user - untuk compatibility)
        public int SelectedShipId => comboShips.SelectedValue != null ? (int)comboShips.SelectedValue : 0;
        public DateTime SelectedETA => dtpETA.Value;
        public DateTime SelectedETD => dtpETD.Value;

        // 'set' properti (Output ke UI)
        public void SetScheduleDataSource(List<BerthAssignment> schedule)
        {
            // Tampilkan di DataGridView
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = schedule;

            // Konfigurasi kolom agar lebih informatif
            if (dgvSchedule.Columns.Count > 0)
            {
                // Sembunyikan kolom ID yang tidak perlu
                if (dgvSchedule.Columns["Id"] != null)
                    dgvSchedule.Columns["Id"].Visible = false;

                // Set header text yang lebih jelas
                if (dgvSchedule.Columns["ShipId"] != null)
                    dgvSchedule.Columns["ShipId"].HeaderText = "Ship ID";
                if (dgvSchedule.Columns["BerthId"] != null)
                    dgvSchedule.Columns["BerthId"].HeaderText = "Berth ID";
                if (dgvSchedule.Columns["ETA"] != null)
                {
                    dgvSchedule.Columns["ETA"].HeaderText = "ETA";
                    dgvSchedule.Columns["ETA"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvSchedule.Columns["ETD"] != null)
                {
                    dgvSchedule.Columns["ETD"].HeaderText = "ETD";
                    dgvSchedule.Columns["ETD"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvSchedule.Columns["Status"] != null)
                    dgvSchedule.Columns["Status"].HeaderText = "Status";
                if (dgvSchedule.Columns["ActualArrivalTime"] != null)
                {
                    dgvSchedule.Columns["ActualArrivalTime"].HeaderText = "Arrival Time";
                    dgvSchedule.Columns["ActualArrivalTime"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvSchedule.Columns["ActualDepartureTime"] != null)
                {
                    dgvSchedule.Columns["ActualDepartureTime"].HeaderText = "Departure Time";
                    dgvSchedule.Columns["ActualDepartureTime"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }

            // Update kartu statistik
            int totalScheduled = schedule.Count;
            int docked = schedule.FindAll(s => s.Status == "Arrived" || s.Status == "Docked" || s.Status == "Berlabuh").Count;
            int waiting = schedule.FindAll(s => s.Status == "Scheduled" || s.Status == "Menunggu").Count;

            lblCardTotalKapalValue.Text = totalScheduled.ToString();
            lblCardSedangBerlabuhValue.Text = docked.ToString();
            lblCardKapalMenungguValue.Text = waiting.ToString();
        }

        public void SetBerthDataSource(List<Berth> berths)
        {
            // Tidak digunakan di dashboard baru, tapi tetap implement untuk interface
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "HarborMaster", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SetCurrentUser(string fullName, bool canOverride)
        {
            // Bisa ditampilkan di header jika perlu
            this.Text = $"HarborMaster - {fullName}";
        }

        // --- Event Handler ---

        private async void MainWindow_Load(object sender, EventArgs e)
        {
            // Panggil Presenter untuk memuat data awal
            await _presenter.LoadInitialDataAsync(_currentUser);

            // Inisialisasi hidden controls untuk compatibility
            comboShips = new ComboBox { Visible = false };
            dtpETA = new DateTimePicker { Visible = false };
            dtpETD = new DateTimePicker { Visible = false };
            this.Controls.Add(comboShips);
            this.Controls.Add(dtpETA);
            this.Controls.Add(dtpETD);
        }

        private void btnAddShip_Click(object sender, EventArgs e)
        {
            // Buka dialog AddShipDialog
            AddShipDialog dialog = new AddShipDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Refresh data setelah berhasil menambah kapal
                _ = _presenter.LoadInitialDataAsync(_currentUser);
            }
        }

        private async void btnRefreshData_Click(object sender, EventArgs e)
        {
            // Refresh data dari database
            await _presenter.LoadInitialDataAsync(_currentUser);
            MessageBox.Show("Data berhasil diperbarui!", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Kembali ke Login
            Application.Restart();
        }

        // --- KODE DESAIN MANUAL ---

        private void InitializeManualUI()
        {
            // Atur Form Utama
            this.ClientSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250); // Background abu-abu terang
            this.Text = "HarborMaster - Dashboard";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // --- Panel Header ---
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "Dashboard Operasional";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Tombol Tambah Kapal (Hijau)
            btnAddShip = new Button();
            btnAddShip.Text = "+ Tambah Kapal";
            btnAddShip.Size = new Size(150, 40);
            btnAddShip.Location = new Point(750, 20);
            btnAddShip.BackColor = Color.FromArgb(76, 175, 80); // Hijau
            btnAddShip.ForeColor = Color.White;
            btnAddShip.FlatStyle = FlatStyle.Flat;
            btnAddShip.FlatAppearance.BorderSize = 0;
            btnAddShip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAddShip.Cursor = Cursors.Hand;
            btnAddShip.Click += btnAddShip_Click;

            // Tombol Refresh Data (Biru)
            btnRefreshData = new Button();
            btnRefreshData.Text = "🔄 Refresh Data";
            btnRefreshData.Size = new Size(140, 40);
            btnRefreshData.Location = new Point(910, 20);
            btnRefreshData.BackColor = Color.FromArgb(33, 150, 243); // Biru
            btnRefreshData.ForeColor = Color.White;
            btnRefreshData.FlatStyle = FlatStyle.Flat;
            btnRefreshData.FlatAppearance.BorderSize = 0;
            btnRefreshData.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefreshData.Cursor = Cursors.Hand;
            btnRefreshData.Click += btnRefreshData_Click;

            // Tombol Kembali (Abu-abu)
            btnBack = new Button();
            btnBack.Text = "← Kembali";
            btnBack.Size = new Size(110, 40);
            btnBack.Location = new Point(1060, 20);
            btnBack.BackColor = Color.FromArgb(158, 158, 158); // Abu-abu
            btnBack.ForeColor = Color.White;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += btnBack_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnAddShip);
            panelHeader.Controls.Add(btnRefreshData);
            panelHeader.Controls.Add(btnBack);

            // --- Kartu 1: Total Kapal (Biru) ---
            cardTotalKapal = new Panel();
            cardTotalKapal.BackColor = Color.FromArgb(52, 152, 219); // Biru
            cardTotalKapal.Size = new Size(350, 120);
            cardTotalKapal.Location = new Point(30, 110);

            lblCardTotalKapalTitle = new Label();
            lblCardTotalKapalTitle.Text = "Total Kapal";
            lblCardTotalKapalTitle.ForeColor = Color.White;
            lblCardTotalKapalTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblCardTotalKapalTitle.Location = new Point(20, 20);
            lblCardTotalKapalTitle.AutoSize = true;

            lblCardTotalKapalValue = new Label();
            lblCardTotalKapalValue.Text = "2";
            lblCardTotalKapalValue.ForeColor = Color.White;
            lblCardTotalKapalValue.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            lblCardTotalKapalValue.Location = new Point(20, 50);
            lblCardTotalKapalValue.AutoSize = true;

            cardTotalKapal.Controls.Add(lblCardTotalKapalTitle);
            cardTotalKapal.Controls.Add(lblCardTotalKapalValue);

            // --- Kartu 2: Sedang Berlabuh (Hijau) ---
            cardSedangBerlabuh = new Panel();
            cardSedangBerlabuh.BackColor = Color.FromArgb(76, 175, 80); // Hijau
            cardSedangBerlabuh.Size = new Size(350, 120);
            cardSedangBerlabuh.Location = new Point(410, 110);

            lblCardSedangBerlabuhTitle = new Label();
            lblCardSedangBerlabuhTitle.Text = "Sedang Berlabuh";
            lblCardSedangBerlabuhTitle.ForeColor = Color.White;
            lblCardSedangBerlabuhTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblCardSedangBerlabuhTitle.Location = new Point(20, 20);
            lblCardSedangBerlabuhTitle.AutoSize = true;

            lblCardSedangBerlabuhValue = new Label();
            lblCardSedangBerlabuhValue.Text = "1";
            lblCardSedangBerlabuhValue.ForeColor = Color.White;
            lblCardSedangBerlabuhValue.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            lblCardSedangBerlabuhValue.Location = new Point(20, 50);
            lblCardSedangBerlabuhValue.AutoSize = true;

            cardSedangBerlabuh.Controls.Add(lblCardSedangBerlabuhTitle);
            cardSedangBerlabuh.Controls.Add(lblCardSedangBerlabuhValue);

            // --- Kartu 3: Kapal Menunggu (Kuning) ---
            cardKapalMenunggu = new Panel();
            cardKapalMenunggu.BackColor = Color.FromArgb(255, 193, 7); // Kuning
            cardKapalMenunggu.Size = new Size(350, 120);
            cardKapalMenunggu.Location = new Point(790, 110);

            lblCardKapalMenungguTitle = new Label();
            lblCardKapalMenungguTitle.Text = "Kapal Menunggu";
            lblCardKapalMenungguTitle.ForeColor = Color.White;
            lblCardKapalMenungguTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblCardKapalMenungguTitle.Location = new Point(20, 20);
            lblCardKapalMenungguTitle.AutoSize = true;

            lblCardKapalMenungguValue = new Label();
            lblCardKapalMenungguValue.Text = "1";
            lblCardKapalMenungguValue.ForeColor = Color.White;
            lblCardKapalMenungguValue.Font = new Font("Segoe UI", 32, FontStyle.Bold);
            lblCardKapalMenungguValue.Location = new Point(20, 50);
            lblCardKapalMenungguValue.AutoSize = true;

            cardKapalMenunggu.Controls.Add(lblCardKapalMenungguTitle);
            cardKapalMenunggu.Controls.Add(lblCardKapalMenungguValue);

            // --- DataGridView untuk Tabel ---
            dgvSchedule = new DataGridView();
            dgvSchedule.Location = new Point(30, 260);
            dgvSchedule.Size = new Size(1140, 400);
            dgvSchedule.BackgroundColor = Color.White;
            dgvSchedule.BorderStyle = BorderStyle.None;
            dgvSchedule.AllowUserToAddRows = false;
            dgvSchedule.AllowUserToDeleteRows = false;
            dgvSchedule.ReadOnly = true;
            dgvSchedule.RowHeadersVisible = false;
            dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSchedule.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSchedule.MultiSelect = false;
            dgvSchedule.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);

            // Styling header
            dgvSchedule.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvSchedule.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSchedule.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSchedule.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvSchedule.ColumnHeadersHeight = 40;
            dgvSchedule.EnableHeadersVisualStyles = false;

            // Styling rows
            dgvSchedule.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvSchedule.DefaultCellStyle.Padding = new Padding(5);
            dgvSchedule.RowTemplate.Height = 35;

            // --- Tambahkan Semua Kontrol ke Form ---
            this.Controls.Add(panelHeader);
            this.Controls.Add(cardTotalKapal);
            this.Controls.Add(cardSedangBerlabuh);
            this.Controls.Add(cardKapalMenunggu);
            this.Controls.Add(dgvSchedule);
        }
    }
}