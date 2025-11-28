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

        public MainWindow(User user)
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!
            _currentUser = user;

            // 3. Buat instance Presenter
            _presenter = new MainPresenter(this);

            // 4. Configure role-based UI visibility
            ConfigureRoleBasedUI();

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

        // ShipOwner specific event handlers
        private void btnAddMyShip_Click(object sender, EventArgs e)
        {
            // Open AddShipDialog for Ship Owner to add their own ship
            AddShipDialog dialog = new AddShipDialog(_currentUser);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Refresh data setelah berhasil menambah kapal
                _ = _presenter.LoadInitialDataAsync(_currentUser);
                MessageBox.Show("Kapal berhasil ditambahkan!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnMyShips_Click(object sender, EventArgs e)
        {
            MyShipsView myShipsView = new MyShipsView(_currentUser);
            myShipsView.ShowDialog();
        }

        private void btnSubmitRequest_Click(object sender, EventArgs e)
        {
            SubmitDockingRequestView submitView = new SubmitDockingRequestView(_currentUser);
            DialogResult result = submitView.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Refresh data after submitting request
                _ = _presenter.LoadInitialDataAsync(_currentUser);
            }
        }

        private void btnMyRequests_Click(object sender, EventArgs e)
        {
            MyRequestsView requestsView = new MyRequestsView(_currentUser);
            requestsView.ShowDialog();
        }

        // Operator specific event handlers
        private void btnPendingRequests_Click(object sender, EventArgs e)
        {
            PendingRequestsView pendingView = new PendingRequestsView(_currentUser);
            pendingView.ShowDialog();

            // Refresh data after closing (in case requests were approved/rejected)
            _ = _presenter.LoadInitialDataAsync(_currentUser);
        }

        private void btnBerthStatus_Click(object sender, EventArgs e)
        {
            BerthStatusView berthView = new BerthStatusView();
            berthView.ShowDialog();
        }

        // HarborMaster specific event handlers
        private void btnStatistics_Click(object sender, EventArgs e)
        {
            HarborMasterStatisticsView statisticsView = new HarborMasterStatisticsView();
            statisticsView.ShowDialog();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            UserProfileWindow profileWindow = new UserProfileWindow(_currentUser);
            profileWindow.ShowDialog();
        }

        /// <summary>
        /// Configure UI elements based on user role
        /// </summary>
        private void ConfigureRoleBasedUI()
        {
            string userRole = _currentUser.Role.ToString();

            // ShipOwner role
            if (userRole == "ShipOwner")
            {
                // Show ShipOwner specific buttons
                btnAddMyShip.Visible = true;
                btnMyShips.Visible = true;
                btnSubmitRequest.Visible = true;
                btnMyRequests.Visible = true;

                // Hide operator/general buttons
                btnAddShip.Visible = false;
                btnPendingRequests.Visible = false;
                btnBerthStatus.Visible = false;

                // Hide HarborMaster exclusive buttons
                btnStatistics.Visible = false;

                // Reposition ShipOwner buttons to avoid overlap (4 buttons now)
                btnAddMyShip.Location = new Point(450, 20);
                btnMyShips.Location = new Point(610, 20);
                btnSubmitRequest.Location = new Point(750, 20);
                btnMyRequests.Location = new Point(910, 20);

                // Update title
                lblTitle.Text = "Ship Owner Dashboard";
            }
            // Operator role
            else if (userRole == "Operator")
            {
                // Hide ShipOwner buttons
                btnAddMyShip.Visible = false;
                btnMyShips.Visible = false;
                btnSubmitRequest.Visible = false;
                btnMyRequests.Visible = false;

                // Show operator buttons (NO Add Ship - operators don't add ships)
                btnAddShip.Visible = false;
                btnPendingRequests.Visible = true;
                btnBerthStatus.Visible = true;

                // Hide HarborMaster exclusive buttons
                btnStatistics.Visible = false;

                // Reposition Operator buttons (only 2 buttons now)
                btnPendingRequests.Location = new Point(500, 20);
                btnBerthStatus.Location = new Point(670, 20);

                lblTitle.Text = "Operator Dashboard";
            }
            // HarborMaster role
            else if (userRole == "HarborMaster")
            {
                // Hide ShipOwner buttons
                btnAddMyShip.Visible = false;
                btnMyShips.Visible = false;
                btnSubmitRequest.Visible = false;
                btnMyRequests.Visible = false;

                // Hide Add Ship - HarborMaster tidak menambah kapal (Ship Owner yang menambahkan)
                btnAddShip.Visible = false;

                // Show operator buttons
                btnPendingRequests.Visible = true;
                btnBerthStatus.Visible = true;

                // Show HarborMaster exclusive buttons
                btnStatistics.Visible = true;

                // Reposition HarborMaster buttons (3 buttons: Pending, Berth, Statistics)
                btnPendingRequests.Location = new Point(500, 20);
                btnBerthStatus.Location = new Point(670, 20);
                btnStatistics.Location = new Point(830, 20);

                lblTitle.Text = "Harbor Master Dashboard";
            }

            // Common buttons always visible and at the end (right side) - adjusted for 1400px width
            btnRefreshData.Visible = true;
            btnRefreshData.Location = new Point(1020, 20);

            btnProfile.Visible = true;
            btnProfile.Location = new Point(1180, 20);

            btnBack.Location = new Point(1240, 20);
        }
    }
}