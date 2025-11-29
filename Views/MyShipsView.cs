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
    /// MyShipsView displays list of ships owned by the logged-in ShipOwner
    /// FULL CRUD: Create (AddShip), Read (ViewDetails), Update (Edit), Delete
    /// </summary>
    public partial class MyShipsView : Form, IMyShipsView
    {
        private readonly MyShipsPresenter _presenter;
        private readonly User _currentUser;

        public MyShipsView(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _presenter = new MyShipsPresenter(this, _currentUser);

            InitializeManualUI();
            this.Load += MyShipsView_Load;
        }

        // --- IMyShipsView Implementation ---

        public void SetShipsDataSource(List<Ship> ships)
        {
            dgvShips.DataSource = null;
            dgvShips.DataSource = ships;

            if (dgvShips.Columns.Count > 0)
            {
                // Hide unnecessary columns
                if (dgvShips.Columns["Id"] != null)
                    dgvShips.Columns["Id"].Visible = false;
                if (dgvShips.Columns["OwnerId"] != null)
                    dgvShips.Columns["OwnerId"].Visible = false;

                // Set header text
                if (dgvShips.Columns["Name"] != null)
                    dgvShips.Columns["Name"].HeaderText = "Nama Kapal";
                if (dgvShips.Columns["ImoNumber"] != null)
                    dgvShips.Columns["ImoNumber"].HeaderText = "IMO Number";
                if (dgvShips.Columns["LengthOverall"] != null)
                {
                    dgvShips.Columns["LengthOverall"].HeaderText = "Panjang (m)";
                    dgvShips.Columns["LengthOverall"].DefaultCellStyle.Format = "N2";
                }
                if (dgvShips.Columns["Draft"] != null)
                {
                    dgvShips.Columns["Draft"].HeaderText = "Draft (m)";
                    dgvShips.Columns["Draft"].DefaultCellStyle.Format = "N2";
                }
                if (dgvShips.Columns["ShipType"] != null)
                    dgvShips.Columns["ShipType"].HeaderText = "Tipe Kapal";
            }

            UpdateActionButtonStates();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "My Ships", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                dgvShips.Enabled = !value;
                btnRefresh.Enabled = !value;
                UpdateActionButtonStates();
            }
        }

        public void CloseView()
        {
            this.Close();
        }

        // --- Event Handlers ---

        private async void MyShipsView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadMyShipsAsync();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await _presenter.LoadMyShipsAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// VIEW DETAILS - Read operation (CRUD - R)
        /// </summary>
        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvShips.SelectedRows.Count == 0)
            {
                ShowError("Pilih kapal yang ingin dilihat detailnya terlebih dahulu.");
                return;
            }

            Ship selectedShip = (Ship)dgvShips.SelectedRows[0].DataBoundItem;

            // Build detailed ship information
            string details = "═══════════════════════════════════════════\n" +
                           "              DETAIL INFORMASI KAPAL\n" +
                           "═══════════════════════════════════════════\n\n" +
                           $"📌 IDENTITAS KAPAL\n" +
                           $"   Nama Kapal    : {selectedShip.Name}\n" +
                           $"   IMO Number    : {selectedShip.ImoNumber}\n" +
                           $"   Tipe Kapal    : {selectedShip.ShipType}\n\n" +
                           $"📏 DIMENSI & SPESIFIKASI\n" +
                           $"   Panjang       : {selectedShip.LengthOverall:N2} meter\n" +
                           $"   Draft         : {selectedShip.Draft:N2} meter\n\n" +
                           $"💰 BIAYA & PRIORITAS\n" +
                           $"   Special Fee   : Rp {selectedShip.CalculateSpecialFee():N0}\n" +
                           $"   Priority Level: {selectedShip.GetPriorityLevel()}/5\n" +
                           $"   Max Docking   : {selectedShip.GetMaxDockingDuration()} hari\n\n" +
                           $"✅ LAYANAN YANG DIBUTUHKAN:\n";

            var services = selectedShip.GetRequiredServices();
            foreach (var service in services)
            {
                details += $"   • {service}\n";
            }

            details += "\n═══════════════════════════════════════════";

            MessageBox.Show(details, $"Detail Kapal - {selectedShip.Name}", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// EDIT - Update operation (CRUD - U)
        /// </summary>
        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvShips.SelectedRows.Count == 0)
            {
                ShowError("Pilih kapal yang ingin di-edit terlebih dahulu.");
                return;
            }

            Ship selectedShip = (Ship)dgvShips.SelectedRows[0].DataBoundItem;

            EditShipDialog dialog = new EditShipDialog(selectedShip, _currentUser);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                await _presenter.LoadMyShipsAsync();
                ShowMessage("Data kapal berhasil diperbarui!");
            }
        }

        /// <summary>
        /// DELETE - Delete operation (CRUD - D)
        /// </summary>
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvShips.SelectedRows.Count == 0)
            {
                ShowError("Pilih kapal yang ingin dihapus terlebih dahulu.");
                return;
            }

            Ship selectedShip = (Ship)dgvShips.SelectedRows[0].DataBoundItem;

            // Build confirmation message with ship details
            string confirmMessage = 
                $"⚠️ KONFIRMASI PENGHAPUSAN DATA KAPAL\n\n" +
                $"Apakah Anda yakin ingin menghapus kapal berikut?\n\n" +
                $"═══════════════════════════════════════════\n" +
                $"Nama    : {selectedShip.Name}\n" +
                $"IMO     : {selectedShip.ImoNumber}\n" +
                $"Tipe    : {selectedShip.ShipType}\n" +
                $"Panjang : {selectedShip.LengthOverall:N2} m\n" +
                $"Draft   : {selectedShip.Draft:N2} m\n" +
                $"═══════════════════════════════════════════\n\n" +
                $"❗ PERINGATAN:\n" +
                $"• Data yang sudah dihapus TIDAK dapat dikembalikan\n" +
                $"• Semua docking request terkait akan dibatalkan\n" +
                $"• History assignment akan tetap tersimpan\n\n" +
                $"Lanjutkan penghapusan?";
            var confirmResult = MessageBox.Show(
                confirmMessage,
                "⚠️ Konfirmasi Hapus Kapal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2  // Default to "No" for safety
            );

            if (confirmResult == DialogResult.Yes)
            {
                await _presenter.DeleteShipAsync(selectedShip.Id);
            }
        }

        /// <summary>
        /// Handle DataGridView selection change
        /// </summary>
        private void dgvShips_SelectionChanged(object sender, EventArgs e)
        {
            UpdateActionButtonStates();
        }

        /// <summary>
        /// Enable/disable action buttons based on selection
        /// </summary>
        private void UpdateActionButtonStates()
        {
            bool hasSelection = dgvShips.SelectedRows.Count > 0;
            bool notLoading = !IsLoading;

            btnViewDetails.Enabled = hasSelection && notLoading;
            btnEdit.Enabled = hasSelection && notLoading;
            btnDelete.Enabled = hasSelection && notLoading;
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings - EXPANDED WIDTH for 5 buttons
            this.ClientSize = new Size(1050, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = $"My Ships - {_currentUser.FullName}";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "📦 Kapal Saya";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // View Details Button (Blue) - READ
            btnViewDetails = new Button();
            btnViewDetails.Text = "👁️ Detail";
            btnViewDetails.Size = new Size(110, 40);
            btnViewDetails.Location = new Point(420, 20);
            btnViewDetails.BackColor = Color.FromArgb(33, 150, 243); // Blue
            btnViewDetails.ForeColor = Color.White;
            btnViewDetails.FlatStyle = FlatStyle.Flat;
            btnViewDetails.FlatAppearance.BorderSize = 0;
            btnViewDetails.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnViewDetails.Cursor = Cursors.Hand;
            btnViewDetails.Click += btnViewDetails_Click;
            btnViewDetails.Enabled = false;

            // Edit Button (Orange) - UPDATE
            btnEdit = new Button();
            btnEdit.Text = "✏️ Edit";
            btnEdit.Size = new Size(110, 40);
            btnEdit.Location = new Point(540, 20);
            btnEdit.BackColor = Color.FromArgb(230, 126, 34); // Orange
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += btnEdit_Click;
            btnEdit.Enabled = false;

            // Delete Button (Red) - DELETE
            btnDelete = new Button();
            btnDelete.Text = "🗑️ Hapus";
            btnDelete.Size = new Size(110, 40);
            btnDelete.Location = new Point(660, 20);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60); // Red
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += btnDelete_Click;
            btnDelete.Enabled = false;

            // Refresh Button (Green)
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(780, 20);
            btnRefresh.BackColor = Color.FromArgb(76, 175, 80); // Green
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += btnRefresh_Click;

            // Close Button (Gray)
            btnClose = new Button();
            btnClose.Text = "✕ Tutup";
            btnClose.Size = new Size(110, 40);
            btnClose.Location = new Point(910, 20);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnViewDetails);
            panelHeader.Controls.Add(btnEdit);
            panelHeader.Controls.Add(btnDelete);
            panelHeader.Controls.Add(btnRefresh);
            panelHeader.Controls.Add(btnClose);

            // DataGridView - EXPANDED WIDTH
            dgvShips = new DataGridView();
            dgvShips.Location = new Point(20, 100);
            dgvShips.Size = new Size(1010, 480);
            dgvShips.BackgroundColor = Color.White;
            dgvShips.BorderStyle = BorderStyle.None;
            dgvShips.AllowUserToAddRows = false;
            dgvShips.AllowUserToDeleteRows = false;
            dgvShips.ReadOnly = true;
            dgvShips.RowHeadersVisible = false;
            dgvShips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvShips.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvShips.MultiSelect = false;
            dgvShips.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvShips.SelectionChanged += dgvShips_SelectionChanged;

            // Styling header
            dgvShips.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvShips.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvShips.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvShips.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgvShips.ColumnHeadersHeight = 40;
            dgvShips.EnableHeadersVisualStyles = false;

            // Styling rows
            dgvShips.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvShips.DefaultCellStyle.Padding = new Padding(5);
            dgvShips.RowTemplate.Height = 35;

            // Loading label
            lblLoading = new Label();
            lblLoading.Text = "⏳ Memuat data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(450, 300);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(dgvShips);
            this.Controls.Add(lblLoading);
        }
    }
}
