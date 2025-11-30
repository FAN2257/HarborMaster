using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    /// <summary>
    /// Booking History View - Shows list of all docking requests
    /// </summary>
    public partial class BookingHistoryView : Form, IBookingHistoryView
    {
        private readonly BookingHistoryPresenter _presenter;
        private readonly User _currentUser;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelSearchFilter;
        private TextBox txtSearch;
        private Button btnClearSearch;
        private ComboBox cboStatus;
        private Label lblResultCount;
        private System.Windows.Forms.Timer searchDebounceTimer;
        private DataGridView dgvBookings;
        private Label lblLoading;
        private Button btnRefresh;
        private Button btnClose;

        public BookingHistoryView(User currentUser)
        {
            _currentUser = currentUser;
            _presenter = new BookingHistoryPresenter(this, _currentUser);

            InitializeComponent();
            InitializeManualUI();
            this.Load += BookingHistoryView_Load;
        }

        // --- IBookingHistoryView Implementation ---

        public void SetBookingsDataSource(List<DockingRequest> bookings)
        {
            dgvBookings.DataSource = null;
            dgvBookings.DataSource = bookings;

            if (dgvBookings.Columns.Count > 0)
            {
                // Hide unnecessary columns
                if (dgvBookings.Columns["Id"] != null)
                    dgvBookings.Columns["Id"].Visible = false;
                if (dgvBookings.Columns["ShipId"] != null)
                    dgvBookings.Columns["ShipId"].Visible = false;
                if (dgvBookings.Columns["OwnerId"] != null)
                    dgvBookings.Columns["OwnerId"].Visible = false;
                if (dgvBookings.Columns["ProcessedBy"] != null)
                    dgvBookings.Columns["ProcessedBy"].Visible = false;
                if (dgvBookings.Columns["BerthAssignmentId"] != null)
                    dgvBookings.Columns["BerthAssignmentId"].Visible = false;
                if (dgvBookings.Columns["CargoType"] != null)
                    dgvBookings.Columns["CargoType"].Visible = false;
                if (dgvBookings.Columns["SpecialRequirements"] != null)
                    dgvBookings.Columns["SpecialRequirements"].Visible = false;
                if (dgvBookings.Columns["ProcessedAt"] != null)
                    dgvBookings.Columns["ProcessedAt"].Visible = false;
                if (dgvBookings.Columns["RejectionReason"] != null)
                    dgvBookings.Columns["RejectionReason"].Visible = false;

                // Set header text
                if (dgvBookings.Columns["CreatedAt"] != null)
                {
                    dgvBookings.Columns["CreatedAt"].HeaderText = "Tanggal Request";
                    dgvBookings.Columns["CreatedAt"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvBookings.Columns["RequestedETA"] != null)
                {
                    dgvBookings.Columns["RequestedETA"].HeaderText = "ETA";
                    dgvBookings.Columns["RequestedETA"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvBookings.Columns["RequestedETD"] != null)
                {
                    dgvBookings.Columns["RequestedETD"].HeaderText = "ETD";
                    dgvBookings.Columns["RequestedETD"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvBookings.Columns["Status"] != null)
                    dgvBookings.Columns["Status"].HeaderText = "Status";
            }
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Booking History", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                dgvBookings.Enabled = !value;
                btnRefresh.Enabled = !value;
            }
        }

        public string SearchTerm => txtSearch?.Text ?? "";

        public string SelectedStatus => cboStatus?.SelectedItem?.ToString() ?? "All Status";

        public void UpdateResultCount(int visibleCount, int totalCount)
        {
            if (lblResultCount != null)
            {
                lblResultCount.Text = $"Showing {visibleCount} of {totalCount} bookings";
                lblResultCount.ForeColor = visibleCount < totalCount
                    ? Color.FromArgb(52, 152, 219)
                    : Color.FromArgb(127, 140, 141);
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        // --- Event Handlers ---

        private async void BookingHistoryView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadBookingsAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadBookingsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvBookings_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DockingRequest selectedRequest = (DockingRequest)dgvBookings.Rows[e.RowIndex].DataBoundItem;

            // Open BookingDetailView
            BookingDetailView detailView = new BookingDetailView(selectedRequest, _currentUser);
            detailView.ShowDialog();

            // Refresh list after closing detail view
            _ = _presenter.LoadBookingsAsync();
        }

        // Search & Filter Event Handlers
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            btnClearSearch.Visible = !string.IsNullOrWhiteSpace(txtSearch.Text);
            searchDebounceTimer.Start();
        }

        private void searchDebounceTimer_Tick(object sender, EventArgs e)
        {
            searchDebounceTimer.Stop();
            _presenter.ApplySearchAndFilter();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.Focus();
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            _presenter.ApplySearchAndFilter();
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = $"Riwayat Pemesanan - {_currentUser.FullName}";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "📋 Riwayat Pemesanan Docking";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(850, 20);
            btnRefresh.BackColor = Color.FromArgb(76, 175, 80);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += btnRefresh_Click;

            // Close Button
            btnClose = new Button();
            btnClose.Text = "✕ Tutup";
            btnClose.Size = new Size(110, 40);
            btnClose.Location = new Point(980, 20);
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnRefresh);
            panelHeader.Controls.Add(btnClose);

            // Search & Filter Panel
            InitializeSearchFilterPanel();

            // DataGridView
            dgvBookings = new DataGridView();
            dgvBookings.Location = new Point(20, 160);
            dgvBookings.Size = new Size(1060, 420);
            dgvBookings.BackgroundColor = Color.White;
            dgvBookings.BorderStyle = BorderStyle.None;
            dgvBookings.AllowUserToAddRows = false;
            dgvBookings.AllowUserToDeleteRows = false;
            dgvBookings.ReadOnly = true;
            dgvBookings.RowHeadersVisible = false;
            dgvBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBookings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBookings.MultiSelect = false;
            dgvBookings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvBookings.CellDoubleClick += dgvBookings_CellDoubleClick;

            // Styling header
            dgvBookings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvBookings.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBookings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBookings.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvBookings.ColumnHeadersHeight = 40;
            dgvBookings.EnableHeadersVisualStyles = false;

            // Styling rows
            dgvBookings.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvBookings.DefaultCellStyle.Padding = new Padding(5);
            dgvBookings.RowTemplate.Height = 35;

            // Loading label
            lblLoading = new Label();
            lblLoading.Text = "⏳ Memuat data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(500, 300);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(panelSearchFilter);
            this.Controls.Add(dgvBookings);
            this.Controls.Add(lblLoading);
        }

        private void InitializeSearchFilterPanel()
        {
            // Main Search & Filter Panel
            panelSearchFilter = new Panel();
            panelSearchFilter.Location = new Point(20, 90);
            panelSearchFilter.Size = new Size(1060, 60);
            panelSearchFilter.BackColor = Color.White;
            panelSearchFilter.BorderStyle = BorderStyle.FixedSingle;

            // Search Icon Label
            Label lblSearchIcon = new Label();
            lblSearchIcon.Text = "🔍";
            lblSearchIcon.Font = new Font("Segoe UI", 14);
            lblSearchIcon.Location = new Point(15, 18);
            lblSearchIcon.Size = new Size(30, 25);
            lblSearchIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Search TextBox
            txtSearch = new TextBox();
            txtSearch.Location = new Point(50, 18);
            txtSearch.Size = new Size(400, 25);
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.PlaceholderText = "Search by request ID...";
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.TextChanged += txtSearch_TextChanged;

            // Clear Search Button
            btnClearSearch = new Button();
            btnClearSearch.Text = "✕";
            btnClearSearch.Location = new Point(455, 17);
            btnClearSearch.Size = new Size(30, 27);
            btnClearSearch.FlatStyle = FlatStyle.Flat;
            btnClearSearch.FlatAppearance.BorderSize = 0;
            btnClearSearch.BackColor = Color.FromArgb(231, 76, 60);
            btnClearSearch.ForeColor = Color.White;
            btnClearSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClearSearch.Cursor = Cursors.Hand;
            btnClearSearch.Click += btnClearSearch_Click;
            btnClearSearch.Visible = false;

            // Filter Label
            Label lblFilter = new Label();
            lblFilter.Text = "Status:";
            lblFilter.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblFilter.Location = new Point(510, 20);
            lblFilter.Size = new Size(60, 20);
            lblFilter.ForeColor = Color.FromArgb(52, 73, 94);

            // Status ComboBox
            cboStatus = new ComboBox();
            cboStatus.Location = new Point(575, 17);
            cboStatus.Size = new Size(150, 25);
            cboStatus.Font = new Font("Segoe UI", 10);
            cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatus.FlatStyle = FlatStyle.Flat;
            cboStatus.Items.AddRange(new object[] {
                "All Status",
                "Pending",
                "Approved",
                "Rejected",
                "Cancelled"
            });
            cboStatus.SelectedIndex = 0;
            cboStatus.SelectedIndexChanged += cboStatus_SelectedIndexChanged;

            // Result Count Label
            lblResultCount = new Label();
            lblResultCount.Text = "Showing 0 of 0 bookings";
            lblResultCount.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblResultCount.Location = new Point(750, 20);
            lblResultCount.Size = new Size(280, 20);
            lblResultCount.ForeColor = Color.FromArgb(127, 140, 141);
            lblResultCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // Search Debounce Timer
            searchDebounceTimer = new System.Windows.Forms.Timer();
            searchDebounceTimer.Interval = 300;
            searchDebounceTimer.Tick += searchDebounceTimer_Tick;

            // Add controls to panel
            panelSearchFilter.Controls.Add(lblSearchIcon);
            panelSearchFilter.Controls.Add(txtSearch);
            panelSearchFilter.Controls.Add(btnClearSearch);
            panelSearchFilter.Controls.Add(lblFilter);
            panelSearchFilter.Controls.Add(cboStatus);
            panelSearchFilter.Controls.Add(lblResultCount);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
