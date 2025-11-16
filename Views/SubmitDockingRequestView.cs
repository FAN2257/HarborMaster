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
    /// SubmitDockingRequestView - Dialog for ShipOwner to submit docking request
    /// </summary>
    public partial class SubmitDockingRequestView : Form, ISubmitDockingRequestView
    {
        private readonly SubmitDockingRequestPresenter _presenter;
        private readonly User _currentUser;

        // UI Controls
        private Panel panelMain;
        private Label lblTitle;
        private Label lblShip;
        private ComboBox cboShip;
        private Label lblETA;
        private DateTimePicker dtpETA;
        private Label lblETD;
        private DateTimePicker dtpETD;
        private Label lblCargoType;
        private TextBox txtCargoType;
        private Label lblSpecialRequirements;
        private TextBox txtSpecialRequirements;
        private Button btnSubmit;
        private Button btnCancel;
        private Label lblError;
        private Label lblSuccess;

        public SubmitDockingRequestView(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _presenter = new SubmitDockingRequestPresenter(this, _currentUser);

            InitializeManualUI();
            this.Load += SubmitDockingRequestView_Load;
        }

        // --- ISubmitDockingRequestView Implementation ---

        public int SelectedShipId => cboShip.SelectedValue != null ? (int)cboShip.SelectedValue : 0;
        public DateTime RequestedETA => dtpETA.Value;
        public DateTime RequestedETD => dtpETD.Value;
        public string CargoType => txtCargoType.Text.Trim();
        public string SpecialRequirements => txtSpecialRequirements.Text.Trim();

        public void SetShipsDataSource(List<Ship> ships)
        {
            cboShip.DataSource = null;
            cboShip.DisplayMember = "Name";
            cboShip.ValueMember = "Id";
            cboShip.DataSource = ships;

            if (ships.Count == 0)
            {
                ShowError("Anda belum memiliki kapal. Silakan tambahkan kapal terlebih dahulu.");
                btnSubmit.Enabled = false;
            }
        }

        public void ShowMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblError.Visible = false;
        }

        public void ShowError(string error)
        {
            lblError.Text = error;
            lblError.Visible = true;
            lblSuccess.Visible = false;
        }

        public bool IsLoading
        {
            get => !btnSubmit.Enabled;
            set
            {
                btnSubmit.Enabled = !value;
                btnCancel.Enabled = !value;
                btnSubmit.Text = value ? "MEMPROSES..." : "KIRIM PERMINTAAN";
            }
        }

        public void CloseView()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // --- Event Handlers ---

        private async void SubmitDockingRequestView_Load(object sender, EventArgs e)
        {
            await _presenter.LoadMyShipsAsync();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validation
            if (SelectedShipId <= 0)
            {
                ShowError("Silakan pilih kapal!");
                return;
            }

            if (RequestedETA >= RequestedETD)
            {
                ShowError("ETA harus lebih awal dari ETD!");
                return;
            }

            if (RequestedETA < DateTime.Now.AddHours(-1))
            {
                ShowError("ETA tidak boleh di masa lalu!");
                return;
            }

            var duration = (RequestedETD - RequestedETA).Days;
            if (duration < 1)
            {
                ShowError("Durasi docking minimal 1 hari!");
                return;
            }

            lblError.Visible = false;
            lblSuccess.Visible = false;

            // Call presenter
            await _presenter.SubmitRequestAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdateDurationDisplay()
        {
            var duration = (RequestedETD - RequestedETA).Days;
            if (duration >= 0)
            {
                lblETD.Text = $"ETD (Estimated Time of Departure) - Durasi: {duration} hari";
            }
        }

        private void dtpETA_ValueChanged(object sender, EventArgs e)
        {
            UpdateDurationDisplay();
        }

        private void dtpETD_ValueChanged(object sender, EventArgs e)
        {
            UpdateDurationDisplay();
        }

        // --- Manual UI Initialization ---

        private void InitializeManualUI()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(89, 171, 227); // Blue background
            this.ClientSize = new Size(600, 550);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Submit Docking Request";

            // Main panel
            panelMain = new Panel();
            panelMain.BackColor = Color.White;
            panelMain.Location = new Point(25, 20);
            panelMain.Size = new Size(550, 510);

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Permintaan Docking Baru";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(130, 15);
            lblTitle.AutoSize = true;

            int currentY = 60;

            // Ship ComboBox
            lblShip = new Label();
            lblShip.Text = "Pilih Kapal *";
            lblShip.Font = new Font("Segoe UI", 9);
            lblShip.ForeColor = Color.Gray;
            lblShip.Location = new Point(30, currentY);
            lblShip.AutoSize = true;

            cboShip = new ComboBox();
            cboShip.Font = new Font("Segoe UI", 10);
            cboShip.Location = new Point(30, currentY + 25);
            cboShip.Size = new Size(490, 30);
            cboShip.DropDownStyle = ComboBoxStyle.DropDownList;
            currentY += 70;

            // ETA
            lblETA = new Label();
            lblETA.Text = "ETA (Estimated Time of Arrival) *";
            lblETA.Font = new Font("Segoe UI", 9);
            lblETA.ForeColor = Color.Gray;
            lblETA.Location = new Point(30, currentY);
            lblETA.AutoSize = true;

            dtpETA = new DateTimePicker();
            dtpETA.Font = new Font("Segoe UI", 10);
            dtpETA.Location = new Point(30, currentY + 25);
            dtpETA.Size = new Size(490, 30);
            dtpETA.Format = DateTimePickerFormat.Custom;
            dtpETA.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpETA.Value = DateTime.Now.AddDays(1);
            dtpETA.ValueChanged += dtpETA_ValueChanged;
            currentY += 70;

            // ETD
            lblETD = new Label();
            lblETD.Text = "ETD (Estimated Time of Departure) - Durasi: 2 hari";
            lblETD.Font = new Font("Segoe UI", 9);
            lblETD.ForeColor = Color.Gray;
            lblETD.Location = new Point(30, currentY);
            lblETD.AutoSize = true;

            dtpETD = new DateTimePicker();
            dtpETD.Font = new Font("Segoe UI", 10);
            dtpETD.Location = new Point(30, currentY + 25);
            dtpETD.Size = new Size(490, 30);
            dtpETD.Format = DateTimePickerFormat.Custom;
            dtpETD.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpETD.Value = DateTime.Now.AddDays(3);
            dtpETD.ValueChanged += dtpETD_ValueChanged;
            currentY += 70;

            // Cargo Type
            lblCargoType = new Label();
            lblCargoType.Text = "Tipe Kargo (Optional)";
            lblCargoType.Font = new Font("Segoe UI", 9);
            lblCargoType.ForeColor = Color.Gray;
            lblCargoType.Location = new Point(30, currentY);
            lblCargoType.AutoSize = true;

            txtCargoType = new TextBox();
            txtCargoType.Font = new Font("Segoe UI", 10);
            txtCargoType.Location = new Point(30, currentY + 25);
            txtCargoType.Size = new Size(490, 30);
            txtCargoType.BorderStyle = BorderStyle.FixedSingle;
            txtCargoType.PlaceholderText = "Contoh: Containers, Crude Oil, Coal";
            currentY += 70;

            // Special Requirements
            lblSpecialRequirements = new Label();
            lblSpecialRequirements.Text = "Kebutuhan Khusus (Optional)";
            lblSpecialRequirements.Font = new Font("Segoe UI", 9);
            lblSpecialRequirements.ForeColor = Color.Gray;
            lblSpecialRequirements.Location = new Point(30, currentY);
            lblSpecialRequirements.AutoSize = true;

            txtSpecialRequirements = new TextBox();
            txtSpecialRequirements.Font = new Font("Segoe UI", 10);
            txtSpecialRequirements.Location = new Point(30, currentY + 25);
            txtSpecialRequirements.Size = new Size(490, 60);
            txtSpecialRequirements.BorderStyle = BorderStyle.FixedSingle;
            txtSpecialRequirements.Multiline = true;
            txtSpecialRequirements.PlaceholderText = "Contoh: Need crane service, Hazmat handling required";
            currentY += 100;

            // Buttons
            btnSubmit = new Button();
            btnSubmit.Text = "KIRIM PERMINTAAN";
            btnSubmit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSubmit.BackColor = Color.FromArgb(76, 175, 80); // Green
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Location = new Point(30, currentY);
            btnSubmit.Size = new Size(235, 45);
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += btnSubmit_Click;

            btnCancel = new Button();
            btnCancel.Text = "BATAL";
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Location = new Point(285, currentY);
            btnCancel.Size = new Size(235, 45);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += btnCancel_Click;
            currentY += 60;

            // Error & Success labels
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = Color.FromArgb(231, 76, 60); // Red
            lblError.Location = new Point(30, currentY);
            lblError.Size = new Size(490, 30);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            lblSuccess = new Label();
            lblSuccess.Text = "";
            lblSuccess.Font = new Font("Segoe UI", 9);
            lblSuccess.ForeColor = Color.FromArgb(76, 175, 80); // Green
            lblSuccess.Location = new Point(30, currentY);
            lblSuccess.Size = new Size(490, 30);
            lblSuccess.TextAlign = ContentAlignment.MiddleCenter;
            lblSuccess.Visible = false;

            // Add controls to panel
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblShip);
            panelMain.Controls.Add(cboShip);
            panelMain.Controls.Add(lblETA);
            panelMain.Controls.Add(dtpETA);
            panelMain.Controls.Add(lblETD);
            panelMain.Controls.Add(dtpETD);
            panelMain.Controls.Add(lblCargoType);
            panelMain.Controls.Add(txtCargoType);
            panelMain.Controls.Add(lblSpecialRequirements);
            panelMain.Controls.Add(txtSpecialRequirements);
            panelMain.Controls.Add(btnSubmit);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(lblError);
            panelMain.Controls.Add(lblSuccess);

            // Add panel to form
            this.Controls.Add(panelMain);
        }
    }
}
