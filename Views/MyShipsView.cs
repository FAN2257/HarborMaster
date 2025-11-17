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
    /// </summary>
    public partial class MyShipsView : Form, IMyShipsView
    {
        private readonly MyShipsPresenter _presenter;
        private readonly User _currentUser;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Button btnEdit;
        private Button btnClose;
        private Button btnRefresh;
        private DataGridView dgvShips;
        private Label lblLoading;

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

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            // Check if a ship is selected
            if (dgvShips.SelectedRows.Count == 0)
            {
                MessageBox.Show("Pilih kapal yang ingin di-edit terlebih dahulu.", "Informasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get selected ship
            Ship selectedShip = (Ship)dgvShips.SelectedRows[0].DataBoundItem;

            // Open EditShipDialog
            EditShipDialog dialog = new EditShipDialog(selectedShip, _currentUser);
            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Refresh the list after successful edit
                await _presenter.LoadMyShipsAsync();
                MessageBox.Show("Data kapal berhasil diperbarui!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(900, 600);
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
            lblTitle.Text = "Kapal Saya";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 25);
            lblTitle.AutoSize = true;

            // Edit Button
            btnEdit = new Button();
            btnEdit.Text = "✏️ Edit";
            btnEdit.Size = new Size(110, 40);
            btnEdit.Location = new Point(530, 20);
            btnEdit.BackColor = Color.FromArgb(230, 126, 34); // Orange
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit.Cursor = Cursors.Hand;
            btnEdit.Click += btnEdit_Click;

            // Refresh Button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 40);
            btnRefresh.Location = new Point(650, 20);
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243); // Blue
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
            btnClose.Location = new Point(780, 20);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(btnEdit);
            panelHeader.Controls.Add(btnRefresh);
            panelHeader.Controls.Add(btnClose);

            // DataGridView
            dgvShips = new DataGridView();
            dgvShips.Location = new Point(20, 100);
            dgvShips.Size = new Size(860, 480);
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
            lblLoading.Text = "Memuat data...";
            lblLoading.Font = new Font("Segoe UI", 12, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(52, 152, 219);
            lblLoading.AutoSize = true;
            lblLoading.Location = new Point(380, 300);
            lblLoading.Visible = false;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(dgvShips);
            this.Controls.Add(lblLoading);
        }
    }
}
