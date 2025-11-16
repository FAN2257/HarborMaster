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
    /// MyRequestsView displays list of docking requests submitted by the ShipOwner
    /// </summary>
    public partial class MyRequestsView : Form, IMyRequestsView
    {
        private readonly MyRequestsPresenter _presenter;
        private readonly User _currentUser;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnRefresh;
        private Button btnCancelRequest;
        private Button btnClose;
        private DataGridView dgvRequests;
        private Label lblLoading;

        public MyRequestsView(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _presenter = new MyRequestsPresenter(this, _currentUser);

            InitializeManualUI();
            this.Load += MyRequestsView_Load;
        }

        // --- IMyRequestsView Implementation ---

        public void SetRequestsDataSource(List<DockingRequest> requests)
        {
            dgvRequests.DataSource = null;

            // Create display data with enriched information
            var displayData = requests.Select(r => new
            {
                r.Id,
                ShipId = r.ShipId,
                ShipName = GetShipName(r.ShipId),
                ETA = r.RequestedETA,
                ETD = r.RequestedETD,
                DurasiHari = r.GetRequestedDays(),
                CargoType = r.CargoType ?? "-",
                SpecialRequirements = r.SpecialRequirements ?? "-",
                Status = r.GetStatusDisplay(),
                StatusRaw = r.Status,
                CreatedAt = r.CreatedAt,
                ProcessedAt = r.ProcessedAt,
                RejectionReason = r.RejectionReason ?? "-"
            }).ToList();

            dgvRequests.DataSource = displayData;

            if (dgvRequests.Columns.Count > 0)
            {
                // Hide unnecessary columns
                if (dgvRequests.Columns["Id"] != null)
                    dgvRequests.Columns["Id"].Visible = false;
                if (dgvRequests.Columns["ShipId"] != null)
                    dgvRequests.Columns["ShipId"].Visible = false;
                if (dgvRequests.Columns["StatusRaw"] != null)
                    dgvRequests.Columns["StatusRaw"].Visible = false;

                // Set header text and formatting
                if (dgvRequests.Columns["ShipName"] != null)
                    dgvRequests.Columns["ShipName"].HeaderText = "Nama Kapal";
                if (dgvRequests.Columns["ETA"] != null)
                {
                    dgvRequests.Columns["ETA"].HeaderText = "ETA";
                    dgvRequests.Columns["ETA"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["ETD"] != null)
                {
                    dgvRequests.Columns["ETD"].HeaderText = "ETD";
                    dgvRequests.Columns["ETD"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["DurasiHari"] != null)
                    dgvRequests.Columns["DurasiHari"].HeaderText = "Durasi (Hari)";
                if (dgvRequests.Columns["CargoType"] != null)
                    dgvRequests.Columns["CargoType"].HeaderText = "Tipe Kargo";
                if (dgvRequests.Columns["SpecialRequirements"] != null)
                    dgvRequests.Columns["SpecialRequirements"].HeaderText = "Kebutuhan Khusus";
                if (dgvRequests.Columns["Status"] != null)
                    dgvRequests.Columns["Status"].HeaderText = "Status";
                if (dgvRequests.Columns["CreatedAt"] != null)
                {
                    dgvRequests.Columns["CreatedAt"].HeaderText = "Tanggal Dibuat";
                    dgvRequests.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["ProcessedAt"] != null)
                {
                    dgvRequests.Columns["ProcessedAt"].HeaderText = "Tanggal Diproses";
                    dgvRequests.Columns["ProcessedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["RejectionReason"] != null)
                    dgvRequests.Columns["RejectionReason"].HeaderText = "Alasan Penolakan";
            }

            // Enable/disable cancel button based on selection
            UpdateCancelButtonState();
        }

        private string GetShipName(int shipId)
        {
            // This is a helper method - in a real scenario, we would get ship name from a lookup
            // For now, we'll just return "Ship #{id}"
            return $"Ship #{shipId}";
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "My Requests", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                dgvRequests.Enabled = !value;
                btnRefresh.Enabled = !value;
                btnCancelRequest.Enabled = !value;
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        // --- Event Handlers ---

        private async void MyRequestsView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadMyRequestsAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadMyRequestsAsync();
        }

        private async void btnCancelRequest_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                ShowError("Silakan pilih permintaan yang ingin dibatalkan!");
                return;
            }

            var selectedRow = dgvRequests.SelectedRows[0];
            var requestId = (int)selectedRow.Cells["Id"].Value;
            var status = (string)selectedRow.Cells["StatusRaw"].Value;

            if (status != "Pending")
            {
                ShowError("Hanya permintaan dengan status 'Pending' yang bisa dibatalkan!");
                return;
            }

            var confirmResult = MessageBox.Show(
                "Apakah Anda yakin ingin membatalkan permintaan ini?",
                "Konfirmasi Pembatalan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                await _presenter.CancelRequestAsync(requestId);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvRequests_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCancelButtonState();
        }

        private void UpdateCancelButtonState()
        {
            if (dgvRequests.SelectedRows.Count > 0)
            {
                var status = dgvRequests.SelectedRows[0].Cells["StatusRaw"].Value?.ToString();
                btnCancelRequest.Enabled = status == "Pending";
            }
            else
            {
                btnCancelRequest.Enabled = false;
            }
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = $"My Docking Requests - {_currentUser.FullName}";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "Permintaan Docking Saya";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(630, 20);
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243); // Blue
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += btnRefresh_Click;

            // Cancel Request Button
            btnCancelRequest = new Button();
            btnCancelRequest.Text = "❌ Batalkan";
            btnCancelRequest.Size = new Size(120, 40);
            btnCancelRequest.Location = new Point(760, 20);
            btnCancelRequest.BackColor = Color.FromArgb(244, 67, 54); // Red
            btnCancelRequest.ForeColor = Color.White;
            btnCancelRequest.FlatStyle = FlatStyle.Flat;
            btnCancelRequest.FlatAppearance.BorderSize = 0;
            btnCancelRequest.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancelRequest.Cursor = Cursors.Hand;
            btnCancelRequest.Click += btnCancelRequest_Click;
            btnCancelRequest.Enabled = false;

            // Close Button
            btnClose = new Button();
            btnClose.Text = "✕ Tutup";
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
            panelHeader.Controls.Add(btnCancelRequest);
            panelHeader.Controls.Add(btnClose);

            // DataGridView
            dgvRequests = new DataGridView();
            dgvRequests.Location = new Point(20, 100);
            dgvRequests.Size = new Size(960, 480);
            dgvRequests.BackgroundColor = Color.White;
            dgvRequests.BorderStyle = BorderStyle.None;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.AllowUserToDeleteRows = false;
            dgvRequests.ReadOnly = true;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.MultiSelect = false;
            dgvRequests.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvRequests.SelectionChanged += dgvRequests_SelectionChanged;

            // Styling header
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvRequests.ColumnHeadersHeight = 40;
            dgvRequests.EnableHeadersVisualStyles = false;

            // Styling rows
            dgvRequests.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvRequests.DefaultCellStyle.Padding = new Padding(5);
            dgvRequests.RowTemplate.Height = 35;

            // Loading label
            lblLoading = new Label();
            lblLoading.Text = "Memuat data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(430, 300);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(dgvRequests);
            this.Controls.Add(lblLoading);
        }
    }
}
