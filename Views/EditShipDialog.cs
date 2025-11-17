using HarborMaster.Models;
using HarborMaster.Repositories;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    /// <summary>
    /// Dialog for editing ship data
    /// Ship Owner can edit their own ships
    /// </summary>
    public partial class EditShipDialog : Form
    {
        private readonly Ship _ship;
        private readonly User _currentUser;
        private readonly ShipRepository _shipRepository;

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
        private Button btnSave;
        private Button btnCancel;
        private Label lblError;

        public EditShipDialog(Ship ship, User currentUser)
        {
            _ship = ship;
            _currentUser = currentUser;
            _shipRepository = new ShipRepository();

            InitializeComponent();
            InitializeManualUI();
            LoadShipData();
        }

        private void LoadShipData()
        {
            txtNamaKapal.Text = _ship.Name;
            txtImoNumber.Text = _ship.ImoNumber;
            txtPanjangKapal.Text = _ship.LengthOverall.ToString();
            txtDraftKapal.Text = _ship.Draft.ToString();
            cboShipType.SelectedItem = _ship.ShipType;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Visible = false;

                // Validate input
                if (string.IsNullOrWhiteSpace(txtNamaKapal.Text))
                {
                    lblError.Text = "Nama kapal tidak boleh kosong!";
                    lblError.Visible = true;
                    return;
                }

                if (cboShipType.SelectedItem == null)
                {
                    lblError.Text = "Pilih tipe kapal!";
                    lblError.Visible = true;
                    return;
                }

                // Parse numbers
                string lengthText = txtPanjangKapal.Text.Trim().Replace(',', '.');
                string draftText = txtDraftKapal.Text.Trim().Replace(',', '.');

                if (!double.TryParse(lengthText, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double length))
                {
                    lblError.Text = "Panjang kapal tidak valid!";
                    lblError.Visible = true;
                    return;
                }

                if (!double.TryParse(draftText, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double draft))
                {
                    lblError.Text = "Draft kapal tidak valid!";
                    lblError.Visible = true;
                    return;
                }

                // Update ship data
                _ship.Name = txtNamaKapal.Text.Trim();
                _ship.ImoNumber = txtImoNumber.Text.Trim();
                _ship.LengthOverall = length;
                _ship.Draft = draft;
                _ship.ShipType = cboShipType.SelectedItem.ToString()!;

                // Validate dimensions
                try
                {
                    _ship.ValidateDimensions();
                }
                catch (ArgumentException ex)
                {
                    lblError.Text = ex.Message;
                    lblError.Visible = true;
                    return;
                }

                // Update in database
                await _shipRepository.UpdateAsync(_ship);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error: {ex.Message}";
                lblError.Visible = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "Edit Ship Data";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main Panel
            panelMain = new Panel();
            panelMain.Location = new Point(30, 30);
            panelMain.Size = new Size(440, 490);
            panelMain.BackColor = Color.White;
            panelMain.Padding = new Padding(20);

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Edit Data Kapal";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;

            int yPos = 70;
            int labelWidth = 120;
            int inputX = labelWidth + 30;
            int inputWidth = 260;
            int rowHeight = 60;

            // Nama Kapal
            lblNamaKapal = new Label();
            lblNamaKapal.Text = "Nama Kapal:";
            lblNamaKapal.Location = new Point(20, yPos);
            lblNamaKapal.Size = new Size(labelWidth, 20);
            lblNamaKapal.Font = new Font("Segoe UI", 10);

            txtNamaKapal = new TextBox();
            txtNamaKapal.Location = new Point(inputX, yPos);
            txtNamaKapal.Size = new Size(inputWidth, 25);
            txtNamaKapal.Font = new Font("Segoe UI", 10);

            yPos += rowHeight;

            // IMO Number
            lblImoNumber = new Label();
            lblImoNumber.Text = "IMO Number:";
            lblImoNumber.Location = new Point(20, yPos);
            lblImoNumber.Size = new Size(labelWidth, 20);
            lblImoNumber.Font = new Font("Segoe UI", 10);

            txtImoNumber = new TextBox();
            txtImoNumber.Location = new Point(inputX, yPos);
            txtImoNumber.Size = new Size(inputWidth, 25);
            txtImoNumber.Font = new Font("Segoe UI", 10);

            yPos += rowHeight;

            // Panjang Kapal
            lblPanjangKapal = new Label();
            lblPanjangKapal.Text = "Panjang (m):";
            lblPanjangKapal.Location = new Point(20, yPos);
            lblPanjangKapal.Size = new Size(labelWidth, 20);
            lblPanjangKapal.Font = new Font("Segoe UI", 10);

            txtPanjangKapal = new TextBox();
            txtPanjangKapal.Location = new Point(inputX, yPos);
            txtPanjangKapal.Size = new Size(inputWidth, 25);
            txtPanjangKapal.Font = new Font("Segoe UI", 10);

            yPos += rowHeight;

            // Draft Kapal
            lblDraftKapal = new Label();
            lblDraftKapal.Text = "Draft (m):";
            lblDraftKapal.Location = new Point(20, yPos);
            lblDraftKapal.Size = new Size(labelWidth, 20);
            lblDraftKapal.Font = new Font("Segoe UI", 10);

            txtDraftKapal = new TextBox();
            txtDraftKapal.Location = new Point(inputX, yPos);
            txtDraftKapal.Size = new Size(inputWidth, 25);
            txtDraftKapal.Font = new Font("Segoe UI", 10);

            yPos += rowHeight;

            // Ship Type
            lblShipType = new Label();
            lblShipType.Text = "Tipe Kapal:";
            lblShipType.Location = new Point(20, yPos);
            lblShipType.Size = new Size(labelWidth, 20);
            lblShipType.Font = new Font("Segoe UI", 10);

            cboShipType = new ComboBox();
            cboShipType.Location = new Point(inputX, yPos);
            cboShipType.Size = new Size(inputWidth, 25);
            cboShipType.Font = new Font("Segoe UI", 10);
            cboShipType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboShipType.Items.AddRange(new string[] { "Container", "Tanker", "Bulk Carrier", "General Cargo", "Passenger" });

            yPos += rowHeight + 10;

            // Error label
            lblError = new Label();
            lblError.ForeColor = Color.FromArgb(231, 76, 60);
            lblError.Location = new Point(20, yPos);
            lblError.Size = new Size(400, 40);
            lblError.Font = new Font("Segoe UI", 9);
            lblError.Visible = false;

            yPos += 50;

            // Save Button
            btnSave = new Button();
            btnSave.Text = "💾 Simpan";
            btnSave.Size = new Size(130, 40);
            btnSave.Location = new Point(150, yPos);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Cursor = Cursors.Hand;
            btnSave.Click += btnSave_Click;

            // Cancel Button
            btnCancel = new Button();
            btnCancel.Text = "Batal";
            btnCancel.Size = new Size(130, 40);
            btnCancel.Location = new Point(290, yPos);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.Click += btnCancel_Click;

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
            panelMain.Controls.Add(lblError);
            panelMain.Controls.Add(btnSave);
            panelMain.Controls.Add(btnCancel);

            // Add panel to form
            this.Controls.Add(panelMain);
        }
    }
}
