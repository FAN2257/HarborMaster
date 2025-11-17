using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;

namespace HarborMaster.Views
{
    public partial class HarborMasterStatisticsView : Form, IStatisticsView
    {
        private readonly StatisticsPresenter _presenter;

        // Header controls
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnRefresh;
        private Button btnClose;
        private Label lblLoading;

        // Statistics cards (Row 1)
        private Panel cardTotalShips;
        private Label lblCardTotalShipsTitle;
        private Label lblCardTotalShipsValue;

        private Panel cardOccupancy;
        private Label lblCardOccupancyTitle;
        private Label lblCardOccupancyValue;

        private Panel cardRevenue;
        private Label lblCardRevenueTitle;
        private Label lblCardRevenueValue;

        private Panel cardPendingRequests;
        private Label lblCardPendingRequestsTitle;
        private Label lblCardPendingRequestsValue;

        // Statistics cards (Row 2)
        private Panel cardCurrentlyDocked;
        private Label lblCardCurrentlyDockedTitle;
        private Label lblCardCurrentlyDockedValue;

        private Panel cardScheduled;
        private Label lblCardScheduledTitle;
        private Label lblCardScheduledValue;

        private Panel cardApprovalRate;
        private Label lblCardApprovalRateTitle;
        private Label lblCardApprovalRateValue;

        private Panel cardAvgRevenue;
        private Label lblCardAvgRevenueTitle;
        private Label lblCardAvgRevenueValue;

        // Tab control for detailed views
        private TabControl tabControl;
        private TabPage tabBerthUtilization;
        private TabPage tabShipTypes;
        private TabPage tabUpcomingSchedule;
        private TabPage tabOperationalMetrics;

        // Berth utilization tab
        private DataGridView dgvBerthUtilization;

        // Ship types tab
        private DataGridView dgvShipTypes;
        private DataGridView dgvRevenueByType;

        // Upcoming schedule tab
        private DataGridView dgvUpcomingArrivals;
        private DataGridView dgvUpcomingDepartures;

        // Operational metrics tab
        private Panel panelMetrics;
        private Label lblAvgDockingDuration;
        private Label lblAvgProcessingTime;
        private Label lblOverdueShips;
        private Label lblMostPopularType;
        private Label lblMostUtilizedBerth;
        private Label lblArrivalsToday;
        private Label lblDeparturesToday;
        private Label lblRevenueToday;

        public HarborMasterStatisticsView()
        {
            InitializeComponent();
            _presenter = new StatisticsPresenter(this);
            InitializeManualUI();
            this.Load += HarborMasterStatisticsView_Load;
        }

        // ===== IStatisticsView Implementation =====

        public bool IsLoading
        {
            get => lblLoading.Visible;
            set
            {
                lblLoading.Visible = value;
                lblLoading.Text = value ? "Loading statistics..." : "";
                btnRefresh.Enabled = !value;
            }
        }

        public string ErrorMessage
        {
            get => "";
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    MessageBox.Show(value, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SetGeneralStatistics(int totalShips, int totalBerths, string occupancyRate, decimal totalRevenue, int pendingRequests)
        {
            lblCardTotalShipsValue.Text = totalShips.ToString();
            lblCardOccupancyValue.Text = occupancyRate;
            lblCardRevenueValue.Text = $"Rp {totalRevenue:N0}";
            lblCardPendingRequestsValue.Text = pendingRequests.ToString();
        }

        public void SetCurrentStatus(int shipsCurrentlyDocked, int shipsScheduled, int shipsDeparted)
        {
            lblCardCurrentlyDockedValue.Text = shipsCurrentlyDocked.ToString();
            lblCardScheduledValue.Text = shipsScheduled.ToString();
        }

        public void SetRequestStatistics(int pendingRequests, int approvedRequests, int rejectedRequests, string approvalRate)
        {
            lblCardApprovalRateValue.Text = approvalRate;
        }

        public void SetShipTypeDistribution(Dictionary<string, int> shipsByType)
        {
            var dataList = shipsByType.Select(kvp => new
            {
                ShipType = kvp.Key,
                Count = kvp.Value,
                Percentage = $"{(kvp.Value * 100.0 / shipsByType.Values.Sum()):F1}%"
            }).OrderByDescending(x => x.Count).ToList();

            dgvShipTypes.DataSource = dataList;

            if (dgvShipTypes.Columns.Count > 0)
            {
                dgvShipTypes.Columns["ShipType"].HeaderText = "Ship Type";
                dgvShipTypes.Columns["Count"].HeaderText = "Count";
                dgvShipTypes.Columns["Percentage"].HeaderText = "Percentage";
            }
        }

        public void SetRevenueByShipType(Dictionary<string, decimal> revenueByType)
        {
            var dataList = revenueByType.Select(kvp => new
            {
                ShipType = kvp.Key,
                Revenue = $"Rp {kvp.Value:N0}",
                Percentage = $"{(kvp.Value * 100.0m / (revenueByType.Values.Sum() == 0 ? 1 : revenueByType.Values.Sum())):F1}%"
            }).OrderByDescending(x => x.Revenue).ToList();

            dgvRevenueByType.DataSource = dataList;

            if (dgvRevenueByType.Columns.Count > 0)
            {
                dgvRevenueByType.Columns["ShipType"].HeaderText = "Ship Type";
                dgvRevenueByType.Columns["Revenue"].HeaderText = "Total Revenue";
                dgvRevenueByType.Columns["Percentage"].HeaderText = "Percentage";
            }
        }

        public void SetBerthUtilization(List<BerthUtilization> berthUtilizations)
        {
            var dataList = berthUtilizations.Select(bu => new
            {
                BerthName = bu.BerthName,
                Assignments = bu.TotalAssignments,
                Utilization = bu.GetUtilizationDisplay(),
                Status = bu.IsCurrentlyOccupied ? "Occupied" : "Available",
                OccupiedUntil = bu.CurrentOccupiedUntil?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                Revenue = $"Rp {bu.TotalRevenue:N0}"
            }).OrderByDescending(x => x.Assignments).ToList();

            dgvBerthUtilization.DataSource = dataList;

            if (dgvBerthUtilization.Columns.Count > 0)
            {
                dgvBerthUtilization.Columns["BerthName"].HeaderText = "Berth";
                dgvBerthUtilization.Columns["Assignments"].HeaderText = "Total Assignments";
                dgvBerthUtilization.Columns["Utilization"].HeaderText = "Utilization Rate";
                dgvBerthUtilization.Columns["Status"].HeaderText = "Current Status";
                dgvBerthUtilization.Columns["OccupiedUntil"].HeaderText = "Occupied Until";
                dgvBerthUtilization.Columns["Revenue"].HeaderText = "Total Revenue";
            }
        }

        public void SetTimeBasedStatistics(int arrivalsToday, int departuresToday, int arrivalsThisWeek, int arrivalsThisMonth, decimal revenueToday, decimal revenueThisWeek, decimal revenueThisMonth)
        {
            lblArrivalsToday.Text = $"Arrivals Today: {arrivalsToday}";
            lblDeparturesToday.Text = $"Departures Today: {departuresToday}";
            lblRevenueToday.Text = $"Revenue Today: Rp {revenueToday:N0}";
        }

        public void SetOperationalMetrics(double averageDockingDuration, double averageProcessingTime, int overdueShips, decimal averageRevenuePerShip)
        {
            lblAvgDockingDuration.Text = $"Average Docking Duration: {averageDockingDuration:F1} days";
            lblAvgProcessingTime.Text = $"Average Request Processing Time: {averageProcessingTime:F1} hours";
            lblOverdueShips.Text = $"Overdue Ships: {overdueShips}";
            lblCardAvgRevenueValue.Text = $"Rp {averageRevenuePerShip:N0}";
        }

        public void SetUpcomingSchedule(List<BerthAssignment> upcomingArrivals, List<BerthAssignment> upcomingDepartures)
        {
            var arrivalsList = upcomingArrivals.Select(a => new
            {
                ShipId = a.ShipId,
                BerthId = a.BerthId,
                ETA = a.ETA.ToString("dd/MM/yyyy HH:mm"),
                ETD = a.ETD.ToString("dd/MM/yyyy HH:mm"),
                Status = a.Status
            }).ToList();

            dgvUpcomingArrivals.DataSource = arrivalsList;

            var departuresList = upcomingDepartures.Select(d => new
            {
                ShipId = d.ShipId,
                BerthId = d.BerthId,
                ETD = d.ETD.ToString("dd/MM/yyyy HH:mm"),
                Status = d.Status
            }).ToList();

            dgvUpcomingDepartures.DataSource = departuresList;
        }

        public void SetTopPerformers(string mostPopularShipType, string mostUtilizedBerth)
        {
            lblMostPopularType.Text = $"Most Popular Ship Type: {mostPopularShipType}";
            lblMostUtilizedBerth.Text = $"Most Utilized Berth: {mostUtilizedBerth}";
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Harbor Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void RefreshCompleted()
        {
            // Can be used to trigger any post-refresh actions
        }

        // ===== Event Handlers =====

        private async void HarborMasterStatisticsView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadStatisticsAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.RefreshStatisticsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ===== UI INITIALIZATION =====

        private void InitializeManualUI()
        {
            this.ClientSize = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "HarborMaster - Statistics Dashboard";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // --- Header Panel ---
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            lblTitle = new Label
            {
                Text = "📊 Harbor Statistics Dashboard",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 25),
                AutoSize = true
            };

            lblLoading = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.White,
                Location = new Point(450, 30),
                AutoSize = true,
                Visible = false
            };

            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Size = new Size(120, 40),
                Location = new Point(1150, 20),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 128, 185),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += btnRefresh_Click;

            btnClose = new Button
            {
                Text = "✕ Close",
                Size = new Size(100, 40),
                Location = new Point(1280, 20),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.AddRange(new Control[] { lblTitle, lblLoading, btnRefresh, btnClose });

            // --- Statistics Cards Row 1 ---
            int cardY = 100;
            int cardWidth = 330;
            int cardHeight = 110;
            int cardSpacing = 20;

            cardTotalShips = CreateStatCard("Total Ships", "0", Color.FromArgb(52, 152, 219), 20, cardY, cardWidth, cardHeight);
            cardOccupancy = CreateStatCard("Berth Occupancy", "0%", Color.FromArgb(46, 204, 113), 20 + cardWidth + cardSpacing, cardY, cardWidth, cardHeight);
            cardRevenue = CreateStatCard("Total Revenue", "Rp 0", Color.FromArgb(155, 89, 182), 20 + (cardWidth + cardSpacing) * 2, cardY, cardWidth, cardHeight);
            cardPendingRequests = CreateStatCard("Pending Requests", "0", Color.FromArgb(241, 196, 15), 20 + (cardWidth + cardSpacing) * 3, cardY, cardWidth, cardHeight);

            lblCardTotalShipsValue = (Label)cardTotalShips.Controls[1];
            lblCardOccupancyValue = (Label)cardOccupancy.Controls[1];
            lblCardRevenueValue = (Label)cardRevenue.Controls[1];
            lblCardPendingRequestsValue = (Label)cardPendingRequests.Controls[1];

            // --- Statistics Cards Row 2 ---
            cardY = 230;

            cardCurrentlyDocked = CreateStatCard("Currently Docked", "0", Color.FromArgb(26, 188, 156), 20, cardY, cardWidth, cardHeight);
            cardScheduled = CreateStatCard("Scheduled Ships", "0", Color.FromArgb(52, 73, 94), 20 + cardWidth + cardSpacing, cardY, cardWidth, cardHeight);
            cardApprovalRate = CreateStatCard("Approval Rate", "0%", Color.FromArgb(230, 126, 34), 20 + (cardWidth + cardSpacing) * 2, cardY, cardWidth, cardHeight);
            cardAvgRevenue = CreateStatCard("Avg Revenue/Ship", "Rp 0", Color.FromArgb(231, 76, 60), 20 + (cardWidth + cardSpacing) * 3, cardY, cardWidth, cardHeight);

            lblCardCurrentlyDockedValue = (Label)cardCurrentlyDocked.Controls[1];
            lblCardScheduledValue = (Label)cardScheduled.Controls[1];
            lblCardApprovalRateValue = (Label)cardApprovalRate.Controls[1];
            lblCardAvgRevenueValue = (Label)cardAvgRevenue.Controls[1];

            // --- Tab Control for Detailed Views ---
            tabControl = new TabControl
            {
                Location = new Point(20, 360),
                Size = new Size(1360, 520),
                Font = new Font("Segoe UI", 10)
            };

            // Tab 1: Berth Utilization
            tabBerthUtilization = new TabPage("⚓ Berth Utilization");
            dgvBerthUtilization = CreateDataGridView(10, 10, 1330, 460);
            tabBerthUtilization.Controls.Add(dgvBerthUtilization);

            // Tab 2: Ship Types
            tabShipTypes = new TabPage("🚢 Ship Analysis");
            dgvShipTypes = CreateDataGridView(10, 10, 650, 460);
            dgvRevenueByType = CreateDataGridView(670, 10, 660, 460);

            var lblShipTypesTitle = new Label
            {
                Text = "Ship Type Distribution",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, -25),
                AutoSize = true
            };
            var lblRevenueByTypeTitle = new Label
            {
                Text = "Revenue by Ship Type",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(670, -25),
                AutoSize = true
            };

            tabShipTypes.Controls.AddRange(new Control[] { dgvShipTypes, dgvRevenueByType });

            // Tab 3: Upcoming Schedule
            tabUpcomingSchedule = new TabPage("📅 Upcoming Schedule");
            dgvUpcomingArrivals = CreateDataGridView(10, 40, 650, 420);
            dgvUpcomingDepartures = CreateDataGridView(670, 40, 660, 420);

            var lblArrivalsTitle = new Label
            {
                Text = "Upcoming Arrivals (Next 7 Days)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            var lblDeparturesTitle = new Label
            {
                Text = "Upcoming Departures (Next 7 Days)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(670, 10),
                AutoSize = true
            };

            tabUpcomingSchedule.Controls.AddRange(new Control[] { lblArrivalsTitle, dgvUpcomingArrivals, lblDeparturesTitle, dgvUpcomingDepartures });

            // Tab 4: Operational Metrics
            tabOperationalMetrics = new TabPage("📈 Operational Metrics");
            panelMetrics = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(1330, 460),
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            int metricY = 20;
            int metricSpacing = 40;

            lblAvgDockingDuration = CreateMetricLabel("Average Docking Duration: N/A", metricY);
            metricY += metricSpacing;
            lblAvgProcessingTime = CreateMetricLabel("Average Request Processing Time: N/A", metricY);
            metricY += metricSpacing;
            lblOverdueShips = CreateMetricLabel("Overdue Ships: 0", metricY);
            metricY += metricSpacing;
            lblArrivalsToday = CreateMetricLabel("Arrivals Today: 0", metricY);
            metricY += metricSpacing;
            lblDeparturesToday = CreateMetricLabel("Departures Today: 0", metricY);
            metricY += metricSpacing;
            lblRevenueToday = CreateMetricLabel("Revenue Today: Rp 0", metricY);
            metricY += metricSpacing;
            lblMostPopularType = CreateMetricLabel("Most Popular Ship Type: N/A", metricY);
            metricY += metricSpacing;
            lblMostUtilizedBerth = CreateMetricLabel("Most Utilized Berth: N/A", metricY);

            panelMetrics.Controls.AddRange(new Control[]
            {
                lblAvgDockingDuration, lblAvgProcessingTime, lblOverdueShips,
                lblArrivalsToday, lblDeparturesToday, lblRevenueToday,
                lblMostPopularType, lblMostUtilizedBerth
            });

            tabOperationalMetrics.Controls.Add(panelMetrics);

            // Add tabs to tab control
            tabControl.TabPages.AddRange(new TabPage[]
            {
                tabBerthUtilization,
                tabShipTypes,
                tabUpcomingSchedule,
                tabOperationalMetrics
            });

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                panelHeader,
                cardTotalShips, cardOccupancy, cardRevenue, cardPendingRequests,
                cardCurrentlyDocked, cardScheduled, cardApprovalRate, cardAvgRevenue,
                tabControl
            });
        }

        private Panel CreateStatCard(string title, string value, Color bgColor, int x, int y, int width, int height)
        {
            var card = new Panel
            {
                BackColor = bgColor,
                Size = new Size(width, height),
                Location = new Point(x, y)
            };

            var lblTitle = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(15, 45),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        private DataGridView CreateDataGridView(int x, int y, int width, int height)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(245, 247, 250) },
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(52, 73, 94),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(5)
                },
                ColumnHeadersHeight = 40,
                EnableHeadersVisualStyles = false,
                DefaultCellStyle =
                {
                    Font = new Font("Segoe UI", 9),
                    Padding = new Padding(5)
                },
                RowTemplate = { Height = 35 }
            };

            return dgv;
        }

        private Label CreateMetricLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 14),
                Location = new Point(20, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }
    }
}
