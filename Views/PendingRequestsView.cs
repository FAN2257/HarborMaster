using HarborMaster.Models;
using HarborMaster.Presenters;
using HarborMaster.Repositories;
using HarborMaster.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    /// <summary>
    /// PendingRequestsView displays all pending docking requests for Operator/HarborMaster to approve or reject
    /// </summary>
    public partial class PendingRequestsView : Form, IPendingRequestsView
    {
        private readonly PendingRequestsPresenter _presenter;
        private readonly User _currentUser;
        private readonly ShipRepository _shipRepo;
        private readonly UserRepository _userRepo;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnApprove;
        private Button btnReject;
        private Button btnRefresh;
        private Button btnClose;
        private DataGridView dgvRequests;
        private Label lblLoading;

        // Cache for ship and user lookups
        private Dictionary<int, string> _shipNameCache = new Dictionary<int, string>();
        private Dictionary<int, string> _ownerNameCache = new Dictionary<int, string>();

        public PendingRequestsView(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _presenter = new PendingRequestsPresenter(this, _currentUser);
            _shipRepo = new ShipRepository();
            _userRepo = new UserRepository();

            InitializeManualUI();
            this.Load += PendingRequestsView_Load;
        }

        // --- IPendingRequestsView Implementation ---

        public async void SetRequestsDataSource(List<DockingRequest> requests)
        {
            // Clear caches
            _shipNameCache.Clear();
            _ownerNameCache.Clear();

            // Fetch all ships and owners in batch for performance
            await LoadLookupsAsync(requests);

            dgvRequests.DataSource = null;

            // Create display data with enriched information
            var displayData = requests.Select(r => new
            {
                r.Id,
                ShipId = r.ShipId,
                ShipName = GetShipName(r.ShipId),
                OwnerName = GetOwnerName(r.OwnerId),
                RequestedETA = r.RequestedETA,
                RequestedETD = r.RequestedETD,
                DurationDays = r.GetRequestedDays(),
                CargoType = r.CargoType ?? "-",
                SpecialRequirements = r.SpecialRequirements ?? "-",
                CreatedDate = r.CreatedAt
            }).ToList();

            dgvRequests.DataSource = displayData;

            if (dgvRequests.Columns.Count > 0)
            {
                // Hide unnecessary columns
                if (dgvRequests.Columns["Id"] != null)
                    dgvRequests.Columns["Id"].Visible = false;
                if (dgvRequests.Columns["ShipId"] != null)
                    dgvRequests.Columns["ShipId"].Visible = false;

                // Set header text and formatting
                if (dgvRequests.Columns["ShipName"] != null)
                    dgvRequests.Columns["ShipName"].HeaderText = "Ship Name";
                if (dgvRequests.Columns["OwnerName"] != null)
                    dgvRequests.Columns["OwnerName"].HeaderText = "Owner Name";
                if (dgvRequests.Columns["RequestedETA"] != null)
                {
                    dgvRequests.Columns["RequestedETA"].HeaderText = "Requested ETA";
                    dgvRequests.Columns["RequestedETA"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["RequestedETD"] != null)
                {
                    dgvRequests.Columns["RequestedETD"].HeaderText = "Requested ETD";
                    dgvRequests.Columns["RequestedETD"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvRequests.Columns["DurationDays"] != null)
                    dgvRequests.Columns["DurationDays"].HeaderText = "Duration (Days)";
                if (dgvRequests.Columns["CargoType"] != null)
                    dgvRequests.Columns["CargoType"].HeaderText = "Cargo Type";
                if (dgvRequests.Columns["SpecialRequirements"] != null)
                    dgvRequests.Columns["SpecialRequirements"].HeaderText = "Special Requirements";
                if (dgvRequests.Columns["CreatedDate"] != null)
                {
                    dgvRequests.Columns["CreatedDate"].HeaderText = "Created Date";
                    dgvRequests.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
            }

            // Enable/disable action buttons based on selection
            UpdateActionButtonStates();
        }

        private async Task LoadLookupsAsync(List<DockingRequest> requests)
        {
            try
            {
                // Get all unique ship IDs and owner IDs
                var shipIds = requests.Select(r => r.ShipId).Distinct().ToList();
                var ownerIds = requests.Select(r => r.OwnerId).Distinct().ToList();

                // Fetch all ships
                var allShips = await _shipRepo.GetAllAsync();
                foreach (var ship in allShips.Where(s => shipIds.Contains(s.Id)))
                {
                    _shipNameCache[ship.Id] = ship.Name;
                }

                // Fetch all owners
                var allUsers = await _userRepo.GetAllAsync();
                foreach (var user in allUsers.Where(u => ownerIds.Contains(u.Id)))
                {
                    _ownerNameCache[user.Id] = user.FullName;
                }
            }
            catch
            {
                // If lookup fails, we'll use fallback values
            }
        }

        private string GetShipName(int shipId)
        {
            return _shipNameCache.ContainsKey(shipId) ? _shipNameCache[shipId] : $"Ship #{shipId}";
        }

        private string GetOwnerName(int ownerId)
        {
            return _ownerNameCache.ContainsKey(ownerId) ? _ownerNameCache[ownerId] : $"User #{ownerId}";
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Pending Requests", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                btnApprove.Enabled = !value && dgvRequests.SelectedRows.Count > 0;
                btnReject.Enabled = !value && dgvRequests.SelectedRows.Count > 0;
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        public async void RefreshData()
        {
            await _presenter.LoadPendingRequestsAsync();
        }

        // --- Event Handlers ---

        private async void PendingRequestsView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadPendingRequestsAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadPendingRequestsAsync();
        }

        private async void btnApprove_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                ShowError("Please select a request to approve!");
                return;
            }

            var selectedRow = dgvRequests.SelectedRows[0];
            var requestId = (int)selectedRow.Cells["Id"].Value;
            var shipName = selectedRow.Cells["ShipName"].Value?.ToString();

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to approve the docking request for {shipName}?\n\nThis will automatically allocate a suitable berth.",
                "Confirm Approval",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                await _presenter.ApproveRequestAsync(requestId);
            }
        }

        private async void btnReject_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                ShowError("Please select a request to reject!");
                return;
            }

            var selectedRow = dgvRequests.SelectedRows[0];
            var requestId = (int)selectedRow.Cells["Id"].Value;
            var shipName = selectedRow.Cells["ShipName"].Value?.ToString();

            // Use InputBox to get rejection reason
            string rejectionReason = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter the reason for rejecting the docking request for {shipName}:",
                "Rejection Reason",
                "",
                -1,
                -1
            );

            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                ShowError("Rejection reason cannot be empty!");
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to reject this request?\n\nReason: {rejectionReason}",
                "Confirm Rejection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmResult == DialogResult.Yes)
            {
                await _presenter.RejectRequestAsync(requestId, rejectionReason);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvRequests_SelectionChanged(object sender, EventArgs e)
        {
            UpdateActionButtonStates();
        }

        private void UpdateActionButtonStates()
        {
            bool hasSelection = dgvRequests.SelectedRows.Count > 0;
            btnApprove.Enabled = hasSelection && !IsLoading;
            btnReject.Enabled = hasSelection && !IsLoading;
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = $"Pending Docking Requests - {_currentUser.FullName}";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "Pending Docking Requests";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Approve Button (Green)
            btnApprove = new Button();
            btnApprove.Text = "✓ Approve";
            btnApprove.Size = new Size(130, 40);
            btnApprove.Location = new Point(750, 20);
            btnApprove.BackColor = Color.FromArgb(76, 175, 80); // Green
            btnApprove.ForeColor = Color.White;
            btnApprove.FlatStyle = FlatStyle.Flat;
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnApprove.Cursor = Cursors.Hand;
            btnApprove.Click += btnApprove_Click;
            btnApprove.Enabled = false;

            // Reject Button (Red)
            btnReject = new Button();
            btnReject.Text = "✗ Reject";
            btnReject.Size = new Size(130, 40);
            btnReject.Location = new Point(890, 20);
            btnReject.BackColor = Color.FromArgb(244, 67, 54); // Red
            btnReject.ForeColor = Color.White;
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnReject.Cursor = Cursors.Hand;
            btnReject.Click += btnReject_Click;
            btnReject.Enabled = false;

            // Refresh Button (Blue)
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(1030, 20);
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
            btnClose.Location = new Point(640, 20);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnClose);
            panelHeader.Controls.Add(btnApprove);
            panelHeader.Controls.Add(btnReject);
            panelHeader.Controls.Add(btnRefresh);

            // DataGridView
            dgvRequests = new DataGridView();
            dgvRequests.Location = new Point(20, 100);
            dgvRequests.Size = new Size(1160, 580);
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
            lblLoading.Text = "Loading data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(530, 350);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(dgvRequests);
            this.Controls.Add(lblLoading);
        }
    }
}
