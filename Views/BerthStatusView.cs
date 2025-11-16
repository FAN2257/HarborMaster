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
    /// <summary>
    /// BerthStatusView displays the status of all berths for Operator/HarborMaster
    /// </summary>
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

        public BerthStatusView()
        {
            InitializeComponent();
            _presenter = new BerthStatusPresenter(this);

            InitializeManualUI();
            this.Load += BerthStatusView_Load;
        }

        // --- IBerthStatusView Implementation ---

        public void SetBerthsDataSource(List<Berth> berths)
        {
            dgvBerths.DataSource = null;

            // Create display data with formatted status
            var displayData = berths.Select(b => new
            {
                b.Id,
                BerthName = b.BerthName,
                Location = b.Location,
                MaxLength = b.MaxLength,
                MaxDraft = b.MaxDraft,
                StatusDisplay = GetStatusWithEmoji(b.Status),
                StatusRaw = b.Status,
                BaseRatePerDay = b.BaseRatePerDay
            }).ToList();

            dgvBerths.DataSource = displayData;

            if (dgvBerths.Columns.Count > 0)
            {
                // Hide unnecessary columns
                if (dgvBerths.Columns["Id"] != null)
                    dgvBerths.Columns["Id"].Visible = false;
                if (dgvBerths.Columns["StatusRaw"] != null)
                    dgvBerths.Columns["StatusRaw"].Visible = false;

                // Set header text and formatting
                if (dgvBerths.Columns["BerthName"] != null)
                {
                    dgvBerths.Columns["BerthName"].HeaderText = "Berth Name";
                    dgvBerths.Columns["BerthName"].Width = 150;
                }
                if (dgvBerths.Columns["Location"] != null)
                {
                    dgvBerths.Columns["Location"].HeaderText = "Location";
                    dgvBerths.Columns["Location"].Width = 200;
                }
                if (dgvBerths.Columns["MaxLength"] != null)
                {
                    dgvBerths.Columns["MaxLength"].HeaderText = "Max Length (m)";
                    dgvBerths.Columns["MaxLength"].DefaultCellStyle.Format = "N2";
                    dgvBerths.Columns["MaxLength"].Width = 120;
                }
                if (dgvBerths.Columns["MaxDraft"] != null)
                {
                    dgvBerths.Columns["MaxDraft"].HeaderText = "Max Draft (m)";
                    dgvBerths.Columns["MaxDraft"].DefaultCellStyle.Format = "N2";
                    dgvBerths.Columns["MaxDraft"].Width = 120;
                }
                if (dgvBerths.Columns["StatusDisplay"] != null)
                {
                    dgvBerths.Columns["StatusDisplay"].HeaderText = "Status";
                    dgvBerths.Columns["StatusDisplay"].Width = 180;
                }
                if (dgvBerths.Columns["BaseRatePerDay"] != null)
                {
                    dgvBerths.Columns["BaseRatePerDay"].HeaderText = "Base Rate Per Day ($)";
                    dgvBerths.Columns["BaseRatePerDay"].DefaultCellStyle.Format = "N2";
                    dgvBerths.Columns["BaseRatePerDay"].Width = 150;
                }
            }

            // Apply row coloring based on status
            ApplyStatusColors();
        }

        private string GetStatusWithEmoji(string status)
        {
            return status switch
            {
                "Available" => "✅ Available",
                "Occupied" => "🚢 Occupied",
                "Maintenance" => "🔧 Maintenance",
                "Damaged" => "⚠️ Damaged",
                _ => status
            };
        }

        private void ApplyStatusColors()
        {
            foreach (DataGridViewRow row in dgvBerths.Rows)
            {
                if (row.Cells["StatusRaw"].Value != null)
                {
                    string status = row.Cells["StatusRaw"].Value.ToString();

                    switch (status)
                    {
                        case "Available":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233); // Light green
                            break;
                        case "Occupied":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224); // Light orange
                            break;
                        case "Maintenance":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(227, 242, 253); // Light blue
                            break;
                        case "Damaged":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238); // Light red
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
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadBerthsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "Berth Status Overview";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "Berth Status";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Refresh Button (Blue)
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(760, 20);
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243); // Blue
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += btnRefresh_Click;

            // Close Button (Gray)
            btnClose = new Button();
            btnClose.Text = "← Close";
            btnClose.Size = new Size(100, 40);
            btnClose.Location = new Point(890, 20);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnRefresh);
            panelHeader.Controls.Add(btnClose);

            // DataGridView
            dgvBerths = new DataGridView();
            dgvBerths.Location = new Point(20, 100);
            dgvBerths.Size = new Size(960, 480);
            dgvBerths.BackgroundColor = Color.White;
            dgvBerths.BorderStyle = BorderStyle.None;
            dgvBerths.AllowUserToAddRows = false;
            dgvBerths.AllowUserToDeleteRows = false;
            dgvBerths.ReadOnly = true;
            dgvBerths.RowHeadersVisible = false;
            dgvBerths.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvBerths.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBerths.MultiSelect = false;

            // Styling header
            dgvBerths.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvBerths.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBerths.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBerths.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvBerths.ColumnHeadersHeight = 40;
            dgvBerths.EnableHeadersVisualStyles = false;

            // Styling rows
            dgvBerths.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvBerths.DefaultCellStyle.Padding = new Padding(5);
            dgvBerths.RowTemplate.Height = 35;

            // Loading label
            lblLoading = new Label();
            lblLoading.Text = "Loading berth data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(420, 300);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(dgvBerths);
            this.Controls.Add(lblLoading);
        }
    }
}
