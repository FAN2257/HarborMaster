using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class BerthStatusView : Form, IBerthStatusView
    {
        private readonly BerthStatusPresenter _presenter;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnRefresh;
        private Button btnClose;
        private DataGridView dgvBerths;
        private Label lblLoading;
        private Button btnMarkArrival;
        private Button btnMarkDeparture;

        public BerthStatusView()
        {
            // InitializeComponent(); // Assuming this is empty or not needed
            _presenter = new BerthStatusPresenter(this);

            InitializeManualUI();
            this.Load += BerthStatusView_Load;
        }

        // --- IBerthStatusView Implementation ---

        public void SetBerthsDataSource(List<BerthStatusViewModel> berths)
        {
            dgvBerths.DataSource = null;
            dgvBerths.DataSource = berths;

            if (dgvBerths.Columns.Count > 0)
            {
                // Hide ID columns
                dgvBerths.Columns["BerthId"].Visible = false;
                dgvBerths.Columns["AssignmentId"].Visible = false;

                // Set header text and formatting
                dgvBerths.Columns["BerthName"].HeaderText = "Berth Name";
                dgvBerths.Columns["BerthStatus"].HeaderText = "Berth Status";
                dgvBerths.Columns["ShipName"].HeaderText = "Ship Name";
                dgvBerths.Columns["AssignmentStatus"].HeaderText = "Assignment Status";
                dgvBerths.Columns["ETA"].DefaultCellStyle.Format = "dd/MM/yy HH:mm";
                dgvBerths.Columns["ETD"].DefaultCellStyle.Format = "dd/MM/yy HH:mm";
                dgvBerths.Columns["ActualArrivalTime"].HeaderText = "Actual Arrival";
                dgvBerths.Columns["ActualArrivalTime"].DefaultCellStyle.Format = "dd/MM/yy HH:mm";
            }

            ApplyStatusColors();
        }

        private void ApplyStatusColors()
        {
            foreach (DataGridViewRow row in dgvBerths.Rows)
            {
                if (row.DataBoundItem is BerthStatusViewModel vm)
                {
                    switch (vm.BerthStatus)
                    {
                        case "Available":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                            break;
                        case "Occupied":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
                            break;
                        default:
                            row.DefaultCellStyle.BackColor = Color.White;
                            break;
                    }
                }
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Berth Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool IsLoading
        {
            get => lblLoading.Visible;
            set
            {
                lblLoading.Visible = value;
                dgvBerths.Enabled = !value;
                btnRefresh.Enabled = !value;
                btnMarkArrival.Enabled = !value;
                btnMarkDeparture.Enabled = !value;
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        // --- Event Handlers ---

        private async void BerthStatusView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadBerthsAsync();
            UpdateSelection(); // Initial button state
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadBerthsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvBerths_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelection();
        }

        private async void btnMarkArrival_Click(object sender, EventArgs e)
        {
            if (dgvBerths.CurrentRow?.DataBoundItem is BerthStatusViewModel selected)
            {
                if (selected.AssignmentId.HasValue)
                {
                    await _presenter.MarkAsArrivedAsync(selected.AssignmentId.Value);
                }
            }
        }

        private async void btnMarkDeparture_Click(object sender, EventArgs e)
        {
            if (dgvBerths.CurrentRow?.DataBoundItem is BerthStatusViewModel selected)
            {
                if (selected.AssignmentId.HasValue)
                {
                    await _presenter.MarkAsDepartedAsync(selected.AssignmentId.Value);
                }
            }
        }

        private void UpdateSelection()
        {
            btnMarkArrival.Enabled = false;
            btnMarkDeparture.Enabled = false;

            if (dgvBerths.CurrentRow?.DataBoundItem is BerthStatusViewModel selected)
            {
                if (selected.AssignmentId.HasValue)
                {
                    if (selected.AssignmentStatus == "Scheduled")
                    {
                        btnMarkArrival.Enabled = true;
                    }
                    else if (selected.AssignmentStatus == "Arrived" || selected.AssignmentStatus == "Docked" || selected.AssignmentStatus == "Berlabuh")
                    {
                        btnMarkDeparture.Enabled = true;
                    }
                }
            }
        }


        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            this.ClientSize = new Size(1200, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "Berth Status Overview";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            panelHeader = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };
            lblTitle = new Label { Text = "Berth Status", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(44, 62, 80), Location = new Point(20, 25), AutoSize = true };

            int buttonX = 400;
            
            btnMarkArrival = new Button { Text = "✓ Catat Kedatangan", Size = new Size(160, 40), Location = new Point(buttonX, 20), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand, Enabled = false };
            btnMarkArrival.FlatAppearance.BorderSize = 0;
            btnMarkArrival.Click += btnMarkArrival_Click;
            buttonX += btnMarkArrival.Width + 10;

            btnMarkDeparture = new Button { Text = "→ Catat Keberangkatan", Size = new Size(180, 40), Location = new Point(buttonX, 20), BackColor = Color.FromArgb(244, 67, 54), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand, Enabled = false };
            btnMarkDeparture.FlatAppearance.BorderSize = 0;
            btnMarkDeparture.Click += btnMarkDeparture_Click;
            buttonX += btnMarkDeparture.Width + 50;

            btnRefresh = new Button { Text = "🔄 Refresh", Size = new Size(120, 40), Location = new Point(buttonX, 20), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += btnRefresh_Click;
            buttonX += btnRefresh.Width + 10;

            btnClose = new Button { Text = "← Close", Size = new Size(100, 40), Location = new Point(buttonX, 20), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold), Cursor = Cursors.Hand };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.AddRange(new Control[] { lblTitle, btnMarkArrival, btnMarkDeparture, btnRefresh, btnClose });

            dgvBerths = new DataGridView { Location = new Point(20, 100), Size = new Size(1160, 480), AllowUserToAddRows = false, AllowUserToDeleteRows = false, ReadOnly = true, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
            dgvBerths.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvBerths.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBerths.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBerths.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvBerths.ColumnHeadersHeight = 40;
            dgvBerths.EnableHeadersVisualStyles = false;
            dgvBerths.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvBerths.DefaultCellStyle.Padding = new Padding(5);
            dgvBerths.RowTemplate.Height = 35;
            dgvBerths.SelectionChanged += dgvBerths_SelectionChanged;

            lblLoading = new Label { Text = "Loading berth data...", Font = new Font("Segoe UI", 12, FontStyle.Italic), ForeColor = Color.FromArgb(52, 152, 219), AutoSize = true, Location = new Point(520, 300), Visible = false };

            this.Controls.AddRange(new Control[] { panelHeader, dgvBerths, lblLoading });
        }
    }
}
