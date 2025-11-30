using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Services;
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
        private readonly WeatherService _weatherService;

        // Weather widget controls
        private Panel panelWeather;
        private Label lblWeatherStatus;
        private Label lblWeatherDetails;
        private Button btnRefreshWeather;

        public MainWindow(User user)
        {
            InitializeComponent(); // ✅ SEKARANG TIDAK KOSONG - UI dibuat via Designer!
            _currentUser = user;

            // 3. Buat instance Presenter dan Services
            _presenter = new MainPresenter(this);
            _weatherService = new WeatherService();

            // 4. Initialize weather widget
            InitializeWeatherWidget();

            // 5. Configure role-based UI visibility
            ConfigureRoleBasedUI();

            // 6. Hubungkan event Form_Load untuk memuat data
            this.Load += MainWindow_Load;
        }

        // --- Implementasi Interface IMainView ---

        // 'get' properti (Input dari user - untuk compatibility)
        public int SelectedShipId => comboShips.SelectedValue != null ? (int)comboShips.SelectedValue : 0;
        public DateTime SelectedETA => dtpETA.Value;
        public DateTime SelectedETD => dtpETD.Value;

        // 'set' properti (Output ke UI)
        public void SetScheduleDataSource(List<DashboardScheduleViewModel> schedule)
        {
            // Tampilkan di DataGridView
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = schedule;

            // Konfigurasi kolom agar lebih informatif
            if (dgvSchedule.Columns.Count > 0)
            {
                var columnsToShow = new Dictionary<string, (string Header, int Width)>
                {
                    { "ShipName", ("Ship Name", 250) },
                    { "BerthName", ("Berth Name", 150) },
                    { "Status", ("Status", 150) },
                    { "ETA", ("ETA", 180) },
                    { "ETD", ("ETD", 180) },
                    { "ActualArrivalTime", ("Actual Arrival", 180) }
                };

                // Sembunyikan semua kolom dulu
                foreach (DataGridViewColumn col in dgvSchedule.Columns)
                {
                    col.Visible = false;
                }

                // Tampilkan dan konfigurasi kolom yang diinginkan
                foreach (var colInfo in columnsToShow)
                {
                    if (dgvSchedule.Columns[colInfo.Key] != null)
                    {
                        var col = dgvSchedule.Columns[colInfo.Key];
                        col.Visible = true;
                        col.HeaderText = colInfo.Value.Header;
                        col.Width = colInfo.Value.Width;
                        if (colInfo.Key.Contains("ETA") || colInfo.Key.Contains("ETD") || colInfo.Key.Contains("Arrival"))
                        {
                            col.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                        }
                    }
                }
            }

            // Update kartu statistik berdasarkan ViewModel
            int totalScheduled = schedule.Count;
            int docked = schedule.Count(s => s.Status == "Arrived" || s.Status == "Docked" || s.Status == "Berlabuh");
            int waiting = schedule.Count(s => s.Status == "Scheduled" || s.Status == "Menunggu");

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

            // Load weather data
            await LoadWeatherDataAsync();
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
            // Open Booking History View (enhanced with invoice and PDF export)
            BookingHistoryView historyView = new BookingHistoryView(_currentUser);
            historyView.ShowDialog();
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
        /// Configure UI elements based on user role - FIXED: No more overlapping buttons
        /// </summary>
        private void ConfigureRoleBasedUI()
        {
            string userRole = _currentUser.Role.ToString();

            // HIDE ALL BUTTONS FIRST to prevent overlap
            btnAddMyShip.Visible = false;
            btnMyShips.Visible = false;
            btnSubmitRequest.Visible = false;
            btnMyRequests.Visible = false;
            btnAddShip.Visible = false;
            btnPendingRequests.Visible = false;
            btnBerthStatus.Visible = false;
            btnStatistics.Visible = false;

            int startX = 450; // Starting X position for role-specific buttons
            int buttonWidth = 140;
            int spacing = 10;
            int currentX = startX;

            // ShipOwner role
            if (userRole == "ShipOwner")
            {
                lblTitle.Text = "Ship Owner Dashboard";

                // Show only ShipOwner buttons
                btnAddMyShip.Visible = true;
                btnAddMyShip.Location = new Point(currentX, 20);
                btnAddMyShip.Text = "➕ Add Ship";
                currentX += buttonWidth + spacing;

                btnMyShips.Visible = true;
                btnMyShips.Location = new Point(currentX, 20);
                btnMyShips.Text = "📦 My Ships";
                currentX += buttonWidth + spacing;

                btnSubmitRequest.Visible = true;
                btnSubmitRequest.Location = new Point(currentX, 20);
                btnSubmitRequest.Text = "📝 Submit Request";
                btnSubmitRequest.Width = 150;
                currentX += 150 + spacing;

                btnMyRequests.Visible = true;
                btnMyRequests.Location = new Point(currentX, 20);
                btnMyRequests.Text = "📋 My Requests";
                currentX += buttonWidth + spacing;
            }
            // Operator role
            else if (userRole == "Operator")
            {
                lblTitle.Text = "Operator Dashboard";

                // Show only Operator buttons
                btnPendingRequests.Visible = true;
                btnPendingRequests.Location = new Point(currentX, 20);
                btnPendingRequests.Text = "⏳ Pending Requests";
                btnPendingRequests.Width = 160;
                currentX += 160 + spacing;

                btnBerthStatus.Visible = true;
                btnBerthStatus.Location = new Point(currentX, 20);
                btnBerthStatus.Text = "⚓ Berth Status";
                currentX += buttonWidth + spacing;
            }
            // HarborMaster role
            else if (userRole == "HarborMaster")
            {
                lblTitle.Text = "Harbor Master Dashboard";

                // Show all Operator buttons + Statistics
                btnPendingRequests.Visible = true;
                btnPendingRequests.Location = new Point(currentX, 20);
                btnPendingRequests.Text = "⏳ Pending Requests";
                btnPendingRequests.Width = 160;
                currentX += 160 + spacing;

                btnBerthStatus.Visible = true;
                btnBerthStatus.Location = new Point(currentX, 20);
                btnBerthStatus.Text = "⚓ Berth Status";
                currentX += buttonWidth + spacing;

                btnStatistics.Visible = true;
                btnStatistics.Location = new Point(currentX, 20);
                btnStatistics.Text = "📊 Statistics";
                currentX += buttonWidth + spacing;
            }

            // Common buttons - always visible at the far right
            currentX += 50; // Extra spacing before common buttons

            btnRefreshData.Visible = true;
            btnRefreshData.Location = new Point(currentX, 20);
            btnRefreshData.Text = "🔄 Refresh";
            btnRefreshData.Width = 120;
            currentX += 120 + spacing;

            btnProfile.Visible = true;
            btnProfile.Location = new Point(currentX, 20);
            btnProfile.Text = "👤 Profile";
            btnProfile.Width = 100;
            currentX += 100 + spacing;

            btnBack.Visible = true;
            btnBack.Location = new Point(currentX, 20);
            btnBack.Text = "← Logout";
            btnBack.Width = 100;
        }

        private void dgvSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        /// <summary>
        /// Initialize weather widget UI
        /// </summary>
        private void InitializeWeatherWidget()
        {
            // Create weather panel (between statistics cards and datagrid)
            panelWeather = new Panel
            {
                Location = new Point(30, 240),
                Size = new Size(1340, 60),
                BackColor = Color.FromArgb(41, 128, 185),
                Visible = true
            };

            // Weather status label (emoji + description)
            lblWeatherStatus = new Label
            {
                Location = new Point(20, 10),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Text = "⏳ Loading weather..."
            };

            // Weather details label (temperature, wind, visibility)
            lblWeatherDetails = new Label
            {
                Location = new Point(20, 35),
                Size = new Size(1100, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(236, 240, 241),
                Text = ""
            };

            // Refresh weather button
            btnRefreshWeather = new Button
            {
                Location = new Point(1220, 15),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Text = "🔄 Refresh",
                Cursor = Cursors.Hand
            };
            btnRefreshWeather.FlatAppearance.BorderSize = 0;
            btnRefreshWeather.Click += async (s, e) => await LoadWeatherDataAsync();

            // Add controls to weather panel
            panelWeather.Controls.Add(lblWeatherStatus);
            panelWeather.Controls.Add(lblWeatherDetails);
            panelWeather.Controls.Add(btnRefreshWeather);

            // Add weather panel to form
            this.Controls.Add(panelWeather);
            panelWeather.BringToFront();

            // Move DataGridView down to make room for weather widget
            dgvSchedule.Location = new Point(30, 310);
            dgvSchedule.Size = new Size(1340, 350);
        }

        /// <summary>
        /// Load weather data from API and update widget
        /// </summary>
        private async System.Threading.Tasks.Task LoadWeatherDataAsync()
        {
            try
            {
                lblWeatherStatus.Text = "⏳ Loading weather...";
                lblWeatherDetails.Text = "";
                btnRefreshWeather.Enabled = false;

                // Fetch weather data
                var weather = await _weatherService.GetHarborWeatherAsync();

                if (weather == null)
                {
                    lblWeatherStatus.Text = "⚠️ Weather service unavailable";
                    lblWeatherDetails.Text = "API key not configured or service error";
                    panelWeather.BackColor = Color.FromArgb(149, 165, 166); // Gray
                    return;
                }

                // Update weather status with emoji and description
                lblWeatherStatus.Text = $"{weather.GetStatusEmoji()} {weather.Description.ToUpper()} at {ApiConfiguration.HarborLocation}";

                // Update weather details
                lblWeatherDetails.Text =
                    $"Temperature: {weather.Temperature:F1}°C (feels like {weather.FeelsLike:F1}°C) | " +
                    $"Wind: {weather.WindSpeedKnots:F1} knots ({weather.WindSpeed:F1} m/s) | " +
                    $"Visibility: {weather.Visibility}m | " +
                    $"Humidity: {weather.Humidity}% | " +
                    $"Wave Height: ~{weather.EstimatedWaveHeight:F1}m";

                // Change panel color based on safety
                if (weather.IsSafeForDocking())
                {
                    panelWeather.BackColor = Color.FromArgb(39, 174, 96); // Green
                }
                else
                {
                    panelWeather.BackColor = Color.FromArgb(231, 76, 60); // Red
                    lblWeatherStatus.Text += " - ⚠️ UNSAFE FOR DOCKING";
                }
            }
            catch (Exception ex)
            {
                lblWeatherStatus.Text = "⚠️ Weather service error";
                lblWeatherDetails.Text = $"Error: {ex.Message}";
                panelWeather.BackColor = Color.FromArgb(149, 165, 166); // Gray

                // Don't show error to user if API key not configured (expected in dev)
                if (!ex.Message.Contains("API key not configured"))
                {
                    MessageBox.Show($"Weather service error: {ex.Message}", "Weather",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnRefreshWeather.Enabled = true;
            }
        }
    }
}