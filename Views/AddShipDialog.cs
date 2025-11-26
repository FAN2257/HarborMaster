using HarborMaster.Presenters;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    public partial class AddShipDialog : Form, IAddShipDialog
    {
        // Presenter
        private readonly ShipArrivalPresenter _presenter;

        // Current user (for Ship Owner)
        private readonly Models.User? _currentUser;

        // UI Controls
        private Panel panelMain;
        private Label lblTitle;
        private Label lblNamaKapal;
        private TextBox txtNamaKapal;
        private Label lblImoNumber;
        private TextBox txtImoNumber;
        private Label lblPanjangKapal;
        private TextBox txtPanjangKapal;
        private Label lblDraftKapal;
        private TextBox txtDraftKapal;
        private Label lblShipType;
        private ComboBox cboShipType;
        private Label lblETA;
        private DateTimePicker dtpETA;
        private Label lblDuration;
        private NumericUpDown nudDuration;
        private Label lblDurationUnit;
        private Label lblETD;
        private Label lblETDValue; // Readonly display
        private Button btnSubmit;
        private Button btnCancel;
        private Label lblError;
        private Label lblSuccess;

        public AddShipDialog(Models.User? user = null)
        {
            InitializeComponent();
            _currentUser = user;
            _presenter = new ShipArrivalPresenter(this);
            InitializeManualUI();

            // Update title based on who is adding the ship
            if (_currentUser != null && _currentUser.Role == Models.UserRole.ShipOwner)
            {
                lblTitle.Text = "Add My Ship";
            }
        }

        // --- Implementasi Interface IAddShipDialog ---

        public string ShipName => txtNamaKapal.Text.Trim();
        public string ImoNumber => txtImoNumber.Text.Trim();
        public int? OwnerId => _currentUser?.Id; // Auto-set dari current user jika ada
        public decimal ShipLength
        {
            get
            {
                // Coba parse dengan InvariantCulture (gunakan titik sebagai desimal)
                // Jika gagal, coba dengan CurrentCulture (koma)
                string text = txtPanjangKapal.Text.Trim().Replace(',', '.');
                if (decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                    return result;
                return 0;
            }
        }
        public decimal ShipDraft
        {
            get
            {
                // Coba parse dengan InvariantCulture (gunakan titik sebagai desimal)
                // Jika gagal, coba dengan CurrentCulture (koma)
                string text = txtDraftKapal.Text.Trim().Replace(',', '.');
                if (decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                    return result;
                return 0;
            }
        }
        public string ShipType => cboShipType.SelectedItem?.ToString() ?? "";
        public DateTime ETA => dtpETA.Value;
        public int DurationDays => (int)nudDuration.Value;
        public DateTime ETD => ETA.AddDays(DurationDays); // Auto-calculate

        public string ErrorMessage
        {
            set
            {
                lblError.Text = value;
                lblError.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public string SuccessMessage
        {
            set
            {
                lblSuccess.Text = value;
                lblSuccess.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public bool IsLoading
        {
            set
            {
                btnSubmit.Enabled = !value;
                btnCancel.Enabled = !value;
                btnSubmit.Text = value ? "MEMPROSES..." : "ALOKASI DERMAGA";
            }
        }

        public void CloseDialog()
        {
            this.Close();
        }

        public void ShowDialogWindow()
        {
            this.ShowDialog();
        }

        public void HandleSuccess(string message)
        {
            SuccessMessage = message;
            ErrorMessage = "";

            // Auto close after 2 seconds
            System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(() =>
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }));
                }
            });
        }

        // --- Event Handlers ---

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(ShipName))
            {
                ErrorMessage = "Nama kapal harus diisi!";
                return;
            }

            // Validate and show parsed values for debugging
            decimal length = ShipLength;
            if (length <= 0)
            {
                ErrorMessage = $"Panjang kapal tidak valid!\nInput: '{txtPanjangKapal.Text}'\nHasil parsing: {length}\n\nGunakan angka tanpa huruf (contoh: 180 atau 180.5)";
                return;
            }

            decimal draft = ShipDraft;
            if (draft <= 0)
            {
                ErrorMessage = $"Draft kapal tidak valid!\nInput: '{txtDraftKapal.Text}'\nHasil parsing: {draft}\n\nGunakan angka tanpa huruf (contoh: 10 atau 10.5)";
                return;
            }

            if (string.IsNullOrWhiteSpace(ShipType))
            {
                ErrorMessage = "Tipe kapal harus dipilih!";
                return;
            }

            if (DurationDays <= 0)
            {
                ErrorMessage = "Durasi berlabuh harus lebih dari 0 hari!";
                return;
            }

            ErrorMessage = "";

            // Call presenter to process arrival
            await _presenter.ProcessArrivalAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdateETDDisplay()
        {
            // Update ETD display saat ETA atau duration berubah
            DateTime calculatedETD = ETA.AddDays(DurationDays);
            lblETDValue.Text = calculatedETD.ToString("dd/MM/yyyy HH:mm");
        }

        private void dtpETA_ValueChanged(object sender, EventArgs e)
        {
            UpdateETDDisplay();
        }

        private void nudDuration_ValueChanged(object sender, EventArgs e)
        {
            UpdateETDDisplay();
        }

        // --- UI Design Manual ---

        private void InitializeManualUI()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(89, 171, 227); // Biru background
            this.ClientSize = new Size(550, 750); // Diperbesar dari 680
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Pendaftaran Kedatangan Kapal";

            // Main panel dengan AutoScroll
            panelMain = new Panel();
            panelMain.BackColor = Color.White;
            panelMain.Location = new Point(25, 20); // Adjusted
            panelMain.Size = new Size(500, 710); // Diperbesar
            panelMain.AutoScroll = true; // Enable scrolling jika masih kurang

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Pendaftaran Kedatangan Kapal";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(80, 15);
            lblTitle.AutoSize = true;

            int currentY = 60;

            // ===== SECTION: DATA KAPAL =====
            Label lblSectionShip = new Label();
            lblSectionShip.Text = "── Data Kapal ──";
            lblSectionShip.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSectionShip.ForeColor = Color.FromArgb(52, 152, 219);
            lblSectionShip.Location = new Point(30, currentY);
            lblSectionShip.AutoSize = true;
            panelMain.Controls.Add(lblSectionShip);
            currentY += 35;

            // Nama Kapal
            lblNamaKapal = new Label();
            lblNamaKapal.Text = "Nama Kapal *";
            lblNamaKapal.Font = new Font("Segoe UI", 9);
            lblNamaKapal.ForeColor = Color.Gray;
            lblNamaKapal.Location = new Point(30, currentY);
            lblNamaKapal.AutoSize = true;

            txtNamaKapal = new TextBox();
            txtNamaKapal.Font = new Font("Segoe UI", 10);
            txtNamaKapal.Location = new Point(30, currentY + 25);
            txtNamaKapal.Size = new Size(410, 30);
            txtNamaKapal.BorderStyle = BorderStyle.FixedSingle;
            currentY += 65;

            // IMO Number
            lblImoNumber = new Label();
            lblImoNumber.Text = "IMO Number (Optional)";
            lblImoNumber.Font = new Font("Segoe UI", 9);
            lblImoNumber.ForeColor = Color.Gray;
            lblImoNumber.Location = new Point(30, currentY);
            lblImoNumber.AutoSize = true;

            txtImoNumber = new TextBox();
            txtImoNumber.Font = new Font("Segoe UI", 10);
            txtImoNumber.Location = new Point(30, currentY + 25);
            txtImoNumber.Size = new Size(410, 30);
            txtImoNumber.BorderStyle = BorderStyle.FixedSingle;
            txtImoNumber.PlaceholderText = "Contoh: IMO 9123456";
            currentY += 65;

            // Panjang & Draft (Side by side)
            lblPanjangKapal = new Label();
            lblPanjangKapal.Text = "Panjang (m) *";
            lblPanjangKapal.Font = new Font("Segoe UI", 9);
            lblPanjangKapal.ForeColor = Color.Gray;
            lblPanjangKapal.Location = new Point(30, currentY);
            lblPanjangKapal.AutoSize = true;

            txtPanjangKapal = new TextBox();
            txtPanjangKapal.Font = new Font("Segoe UI", 10);
            txtPanjangKapal.Location = new Point(30, currentY + 25);
            txtPanjangKapal.Size = new Size(195, 30);
            txtPanjangKapal.BorderStyle = BorderStyle.FixedSingle;
            txtPanjangKapal.PlaceholderText = "Contoh: 180";

            lblDraftKapal = new Label();
            lblDraftKapal.Text = "Draft (m) *";
            lblDraftKapal.Font = new Font("Segoe UI", 9);
            lblDraftKapal.ForeColor = Color.Gray;
            lblDraftKapal.Location = new Point(245, currentY);
            lblDraftKapal.AutoSize = true;

            txtDraftKapal = new TextBox();
            txtDraftKapal.Font = new Font("Segoe UI", 10);
            txtDraftKapal.Location = new Point(245, currentY + 25);
            txtDraftKapal.Size = new Size(195, 30);
            txtDraftKapal.BorderStyle = BorderStyle.FixedSingle;
            txtDraftKapal.PlaceholderText = "Contoh: 10";
            currentY += 65;

            // Ship Type Dropdown
            lblShipType = new Label();
            lblShipType.Text = "Tipe Kapal *";
            lblShipType.Font = new Font("Segoe UI", 9);
            lblShipType.ForeColor = Color.Gray;
            lblShipType.Location = new Point(30, currentY);
            lblShipType.AutoSize = true;

            cboShipType = new ComboBox();
            cboShipType.Font = new Font("Segoe UI", 10);
            cboShipType.Location = new Point(30, currentY + 25);
            cboShipType.Size = new Size(410, 30);
            cboShipType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboShipType.Items.AddRange(new object[] {
                "Container",
                "Tanker",
                "Bulk Carrier",
                "General Cargo",
                "Passenger"
            });
            cboShipType.SelectedIndex = 0;
            currentY += 80;

            // ===== SECTION: JADWAL KEDATANGAN =====
            Label lblSectionSchedule = new Label();
            lblSectionSchedule.Text = "── Jadwal Kedatangan ──";
            lblSectionSchedule.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSectionSchedule.ForeColor = Color.FromArgb(52, 152, 219);
            lblSectionSchedule.Location = new Point(30, currentY);
            lblSectionSchedule.AutoSize = true;
            panelMain.Controls.Add(lblSectionSchedule);
            currentY += 35;

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
            dtpETA.Size = new Size(410, 30);
            dtpETA.Format = DateTimePickerFormat.Custom;
            dtpETA.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpETA.ValueChanged += dtpETA_ValueChanged;
            currentY += 65;

            // Duration
            lblDuration = new Label();
            lblDuration.Text = "Durasi Berlabuh *";
            lblDuration.Font = new Font("Segoe UI", 9);
            lblDuration.ForeColor = Color.Gray;
            lblDuration.Location = new Point(30, currentY);
            lblDuration.AutoSize = true;

            nudDuration = new NumericUpDown();
            nudDuration.Font = new Font("Segoe UI", 10);
            nudDuration.Location = new Point(30, currentY + 25);
            nudDuration.Size = new Size(120, 30);
            nudDuration.Minimum = 1;
            nudDuration.Maximum = 30;
            nudDuration.Value = 2;
            nudDuration.ValueChanged += nudDuration_ValueChanged;

            lblDurationUnit = new Label();
            lblDurationUnit.Text = "hari";
            lblDurationUnit.Font = new Font("Segoe UI", 10);
            lblDurationUnit.ForeColor = Color.Gray;
            lblDurationUnit.Location = new Point(160, currentY + 30);
            lblDurationUnit.AutoSize = true;
            currentY += 65;

            // ETD (Calculated/Readonly)
            lblETD = new Label();
            lblETD.Text = "ETD (Estimated Time of Departure)";
            lblETD.Font = new Font("Segoe UI", 9);
            lblETD.ForeColor = Color.Gray;
            lblETD.Location = new Point(30, currentY);
            lblETD.AutoSize = true;

            lblETDValue = new Label();
            lblETDValue.Text = dtpETA.Value.AddDays(2).ToString("dd/MM/yyyy HH:mm");
            lblETDValue.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblETDValue.ForeColor = Color.FromArgb(52, 152, 219);
            lblETDValue.Location = new Point(30, currentY + 25);
            lblETDValue.AutoSize = true;
            currentY += 70;

            // Buttons
            btnSubmit = new Button();
            btnSubmit.Text = "ALOKASI DERMAGA";
            btnSubmit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSubmit.BackColor = Color.FromArgb(76, 175, 80); // Hijau
            btnSubmit.ForeColor = Color.White;
            btnSubmit.FlatStyle = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Location = new Point(30, currentY);
            btnSubmit.Size = new Size(195, 45);
            btnSubmit.Cursor = Cursors.Hand;
            btnSubmit.Click += btnSubmit_Click;

            btnCancel = new Button();
            btnCancel.Text = "BATAL";
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158); // Abu-abu
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Location = new Point(245, currentY);
            btnCancel.Size = new Size(195, 45);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += btnCancel_Click;
            currentY += 60;

            // Error & Success labels
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = Color.FromArgb(231, 76, 60); // Red
            lblError.Location = new Point(30, currentY);
            lblError.Size = new Size(410, 40);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            lblSuccess = new Label();
            lblSuccess.Text = "";
            lblSuccess.Font = new Font("Segoe UI", 9);
            lblSuccess.ForeColor = Color.FromArgb(76, 175, 80); // Green
            lblSuccess.Location = new Point(30, currentY);
            lblSuccess.Size = new Size(410, 40);
            lblSuccess.TextAlign = ContentAlignment.MiddleCenter;
            lblSuccess.Visible = false;

            // Add controls to panel
            panelMain.Controls.Add(lblTitle);
            panelMain.Controls.Add(lblNamaKapal);
            panelMain.Controls.Add(txtNamaKapal);
            panelMain.Controls.Add(lblImoNumber);
            panelMain.Controls.Add(txtImoNumber);
            panelMain.Controls.Add(lblPanjangKapal);
            panelMain.Controls.Add(txtPanjangKapal);
            panelMain.Controls.Add(lblDraftKapal);
            panelMain.Controls.Add(txtDraftKapal);
            panelMain.Controls.Add(lblShipType);
            panelMain.Controls.Add(cboShipType);
            panelMain.Controls.Add(lblETA);
            panelMain.Controls.Add(dtpETA);
            panelMain.Controls.Add(lblDuration);
            panelMain.Controls.Add(nudDuration);
            panelMain.Controls.Add(lblDurationUnit);
            panelMain.Controls.Add(lblETD);
            panelMain.Controls.Add(lblETDValue);
            panelMain.Controls.Add(btnSubmit);
            panelMain.Controls.Add(btnCancel);
            panelMain.Controls.Add(lblError);
            panelMain.Controls.Add(lblSuccess);

            // Add panel to form
            this.Controls.Add(panelMain);
        }

        private void AddShipDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
