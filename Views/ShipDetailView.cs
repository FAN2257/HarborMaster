using HarborMaster.Models;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    /// <summary>
    /// Ship Detail View - Shows detailed information about a ship
    /// Implements PDF export functionality
    /// </summary>
    public partial class ShipDetailView : Form, IShipDetailView
    {
        private readonly PdfGenerationService _pdfService;
        private Ship? _currentShip;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelContent;
        private Label lblIdentity;
        private Label lblShipInfo;
        private Label lblDimensions;
        private Label lblDimensionInfo;
        private Label lblFees;
        private Label lblFeeInfo;
        private Label lblServices;
        private Label lblServicesList;
        private Button btnExportPdf;
        private Button btnClose;

        public ShipDetailView()
        {
            _pdfService = new PdfGenerationService();
            InitializeComponent();
            InitializeManualUI();
        }

        // --- IShipDetailView Implementation ---

        public void SetShipDetails(Ship ship)
        {
            _currentShip = ship;
            DisplayShipInformation();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Ship Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool IsExporting
        {
            get => !btnExportPdf.Enabled;
            set
            {
                btnExportPdf.Enabled = !value;
                btnExportPdf.Text = value ? "⏳ Exporting..." : "📄 Export to PDF";
            }
        }

        // --- Private Methods ---

        private void DisplayShipInformation()
        {
            if (_currentShip == null) return;

            this.Text = $"Ship Detail - {_currentShip.Name}";

            // Ship Identity
            lblShipInfo.Text =
                $"Nama Kapal     : {_currentShip.Name}\n" +
                $"IMO Number     : {_currentShip.ImoNumber}\n" +
                $"Tipe Kapal     : {_currentShip.ShipType}";

            // Dimensions
            lblDimensionInfo.Text =
                $"Panjang        : {_currentShip.LengthOverall:N2} meter\n" +
                $"Draft          : {_currentShip.Draft:N2} meter";

            // Fees & Priority
            lblFeeInfo.Text =
                $"Special Fee    : Rp {_currentShip.CalculateSpecialFee():N0}\n" +
                $"Priority Level : {_currentShip.GetPriorityLevel()}/5\n" +
                $"Max Docking    : {_currentShip.GetMaxDockingDuration()} hari";

            // Services
            var services = _currentShip.GetRequiredServices();
            lblServicesList.Text = string.Join("\n", services.ConvertAll(s => $"• {s}"));
        }

        private async void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (_currentShip == null)
            {
                ShowError("No ship data to export!");
                return;
            }

            try
            {
                IsExporting = true;

                // Generate PDF in background
                await System.Threading.Tasks.Task.Run(() =>
                {
                    string pdfPath = _pdfService.GenerateShipDetailPdf(_currentShip);

                    // Open PDF
                    PdfGenerationService.OpenPdfFile(pdfPath);

                    // Show success message
                    this.Invoke(new Action(() =>
                    {
                        ShowMessage($"PDF berhasil dibuat!\n\nFile: {System.IO.Path.GetFileName(pdfPath)}\nLokasi: Downloads folder\n\nFile akan dibuka secara otomatis.");
                    }));
                });
            }
            catch (Exception ex)
            {
                ShowError($"Gagal membuat PDF: {ex.Message}");
            }
            finally
            {
                IsExporting = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeManualUI()
        {
            // Form settings
            this.ClientSize = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "Ship Detail";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(52, 73, 94);

            lblTitle = new Label();
            lblTitle.Text = "📋 DETAIL INFORMASI KAPAL";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            panelHeader.Controls.Add(lblTitle);

            // Content Panel
            panelContent = new Panel();
            panelContent.Location = new Point(20, 80);
            panelContent.Size = new Size(560, 540);
            panelContent.BackColor = Color.White;
            panelContent.BorderStyle = BorderStyle.FixedSingle;
            panelContent.AutoScroll = true;

            int yPos = 20;

            // Identity Section
            lblIdentity = CreateSectionHeader("📌 IDENTITAS KAPAL", yPos);
            yPos += 35;
            lblShipInfo = CreateInfoLabel(yPos);
            yPos += 80;

            // Dimensions Section
            lblDimensions = CreateSectionHeader("📏 DIMENSI & SPESIFIKASI", yPos);
            yPos += 35;
            lblDimensionInfo = CreateInfoLabel(yPos);
            yPos += 60;

            // Fees Section
            lblFees = CreateSectionHeader("💰 BIAYA & PRIORITAS", yPos);
            yPos += 35;
            lblFeeInfo = CreateInfoLabel(yPos);
            yPos += 80;

            // Services Section
            lblServices = CreateSectionHeader("✅ LAYANAN YANG DIBUTUHKAN", yPos);
            yPos += 35;
            lblServicesList = CreateInfoLabel(yPos);

            panelContent.Controls.Add(lblIdentity);
            panelContent.Controls.Add(lblShipInfo);
            panelContent.Controls.Add(lblDimensions);
            panelContent.Controls.Add(lblDimensionInfo);
            panelContent.Controls.Add(lblFees);
            panelContent.Controls.Add(lblFeeInfo);
            panelContent.Controls.Add(lblServices);
            panelContent.Controls.Add(lblServicesList);

            // Export PDF Button
            btnExportPdf = new Button();
            btnExportPdf.Text = "📄 Export to PDF";
            btnExportPdf.Size = new Size(200, 45);
            btnExportPdf.Location = new Point(150, 630);
            btnExportPdf.BackColor = Color.FromArgb(76, 175, 80); // Green
            btnExportPdf.ForeColor = Color.White;
            btnExportPdf.FlatStyle = FlatStyle.Flat;
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnExportPdf.Cursor = Cursors.Hand;
            btnExportPdf.Click += btnExportPdf_Click;

            // Close Button
            btnClose = new Button();
            btnClose.Text = "✕ Tutup";
            btnClose.Size = new Size(150, 45);
            btnClose.Location = new Point(360, 630);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(panelContent);
            this.Controls.Add(btnExportPdf);
            this.Controls.Add(btnClose);
        }

        private Label CreateSectionHeader(string text, int yPos)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(52, 73, 94);
            lbl.Location = new Point(15, yPos);
            lbl.Size = new Size(520, 25);
            lbl.BackColor = Color.FromArgb(236, 240, 241);
            lbl.Padding = new Padding(10, 5, 5, 5);
            return lbl;
        }

        private Label CreateInfoLabel(int yPos)
        {
            Label lbl = new Label();
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.FromArgb(44, 62, 80);
            lbl.Location = new Point(30, yPos);
            lbl.Size = new Size(500, 60);
            lbl.AutoSize = false;
            return lbl;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
