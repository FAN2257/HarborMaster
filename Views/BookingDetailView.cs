using HarborMaster.Models;
using HarborMaster.Repositories;
using HarborMaster.Services;
using HarborMaster.Views.Interfaces;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HarborMaster.Views
{
    /// <summary>
    /// Booking Detail View - Shows detailed information about a docking request
    /// Includes invoice information and PDF export capability
    /// </summary>
    public partial class BookingDetailView : Form, IBookingDetailView
    {
        private readonly PdfGenerationService _pdfService;
        private readonly User _currentUser;
        private DockingRequest? _currentRequest;
        private Ship? _ship;
        private Berth? _berth;
        private BerthAssignment? _assignment;
        private Invoice? _invoice;

        // UI Controls
        private Panel panelHeader;
        private Label lblTitle;
        private Panel scrollContent;
        private Button btnDownloadPdf;
        private Button btnClose;

        // Section labels
        private Label lblRequestInfo, lblShipInfo, lblBerthInfo, lblInvoiceInfo;

        public BookingDetailView(DockingRequest request, User currentUser)
        {
            _currentRequest = request;
            _currentUser = currentUser;
            _pdfService = new PdfGenerationService();

            InitializeComponent();
            InitializeManualUI();
            this.Load += BookingDetailView_Load;
        }

        // --- IBookingDetailView Implementation ---

        public void SetBookingDetails(DockingRequest request, Ship ship, Berth? berth, BerthAssignment? assignment, Invoice? invoice)
        {
            _currentRequest = request;
            _ship = ship;
            _berth = berth;
            _assignment = assignment;
            _invoice = invoice;

            DisplayBookingInformation();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Booking Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public bool IsExporting
        {
            get => !btnDownloadPdf.Enabled;
            set
            {
                btnDownloadPdf.Enabled = !value;
                btnDownloadPdf.Text = value ? "⏳ Generating..." : "📄 Download Invoice PDF";
            }
        }

        // --- Private Methods ---

        private async void BookingDetailView_Load(object sender, EventArgs e)
        {
            if (_currentRequest == null) return;

            try
            {
                // Load ship data
                var shipRepo = new ShipRepository();
                _ship = await shipRepo.GetByIdAsync(_currentRequest.ShipId);

                // Load berth assignment if approved
                if (_currentRequest.BerthAssignmentId.HasValue)
                {
                    var assignmentRepo = new BerthAssignmentRepository();
                    _assignment = await assignmentRepo.GetByIdAsync(_currentRequest.BerthAssignmentId.Value);

                    if (_assignment != null)
                    {
                        // Load berth data
                        var berthRepo = new BerthRepository();
                        _berth = await berthRepo.GetByIdAsync(_assignment.BerthId);

                        // Load invoice
                        var invoiceRepo = new InvoiceRepository();
                        var allInvoices = await invoiceRepo.GetAllAsync();
                        _invoice = allInvoices.Find(inv => inv.BerthAssignmentId == _assignment.Id);
                    }
                }

                DisplayBookingInformation();
            }
            catch (Exception ex)
            {
                ShowError($"Error loading booking details: {ex.Message}");
            }
        }

        private void DisplayBookingInformation()
        {
            if (_currentRequest == null) return;

            int yPos = 20;

            // Clear existing content
            scrollContent.Controls.Clear();

            // Request Information Section
            var lblRequestHeader = CreateSectionHeader("📋 INFORMASI PEMESANAN", yPos);
            scrollContent.Controls.Add(lblRequestHeader);
            yPos += 35;

            lblRequestInfo = CreateInfoLabel(yPos);
            lblRequestInfo.Text =
                $"Request ID      : REQ-{_currentRequest.Id:D4}\n" +
                $"Status          : {_currentRequest.GetStatusDisplay()}\n" +
                $"Tanggal Request : {_currentRequest.CreatedAt:dd MMMM yyyy HH:mm}\n" +
                $"Durasi          : {_currentRequest.GetRequestedDays()} hari";
            lblRequestInfo.Height = 100;
            scrollContent.Controls.Add(lblRequestInfo);
            yPos += 110;

            // Ship Information Section
            if (_ship != null)
            {
                var lblShipHeader = CreateSectionHeader("🚢 INFORMASI KAPAL", yPos);
                scrollContent.Controls.Add(lblShipHeader);
                yPos += 35;

                lblShipInfo = CreateInfoLabel(yPos);
                lblShipInfo.Text =
                    $"Nama Kapal      : {_ship.Name}\n" +
                    $"IMO Number      : {_ship.ImoNumber}\n" +
                    $"Tipe Kapal      : {_ship.ShipType}\n" +
                    $"Panjang / Draft : {_ship.LengthOverall:N2} m / {_ship.Draft:N2} m";
                lblShipInfo.Height = 100;
                scrollContent.Controls.Add(lblShipInfo);
                yPos += 110;
            }

            // Berth Information Section
            if (_assignment != null && _berth != null)
            {
                var lblBerthHeader = CreateSectionHeader("⚓ INFORMASI DERMAGA", yPos);
                scrollContent.Controls.Add(lblBerthHeader);
                yPos += 35;

                lblBerthInfo = CreateInfoLabel(yPos);
                string actualArrival = _assignment.ActualArrivalTime.HasValue
                    ? _assignment.ActualArrivalTime.Value.ToString("dd/MM/yyyy HH:mm")
                    : "-";
                string actualDeparture = _assignment.ActualDepartureTime.HasValue
                    ? _assignment.ActualDepartureTime.Value.ToString("dd/MM/yyyy HH:mm")
                    : "-";

                lblBerthInfo.Text =
                    $"Berth Assigned  : {_berth.BerthName}\n" +
                    $"Location        : {_berth.Location}\n" +
                    $"ETA             : {_assignment.ETA:dd/MM/yyyy HH:mm}\n" +
                    $"ETD             : {_assignment.ETD:dd/MM/yyyy HH:mm}\n" +
                    $"Actual Arrival  : {actualArrival}\n" +
                    $"Actual Depart   : {actualDeparture}\n" +
                    $"Status          : {_assignment.Status}";
                lblBerthInfo.Height = 160;
                scrollContent.Controls.Add(lblBerthInfo);
                yPos += 170;
            }

            // Invoice Section
            if (_invoice != null && _ship != null && _assignment != null)
            {
                var lblInvoiceHeader = CreateSectionHeader("💰 INVOICE & PEMBAYARAN", yPos);
                scrollContent.Controls.Add(lblInvoiceHeader);
                yPos += 35;

                // Calculate fees
                var duration = (_assignment.ETD - _assignment.ETA).Days;
                var berthFeePerDay = 500000m;
                var berthFeeTotal = berthFeePerDay * duration;
                var specialFee = _ship.CalculateSpecialFee();
                var serviceFee = 750000m;
                var subtotal = berthFeeTotal + specialFee + serviceFee;
                var taxRate = 0.11m;
                var taxAmount = subtotal * taxRate;
                var totalAmount = subtotal + taxAmount;

                string paymentStatus = _invoice.IsPaid ? "✅ PAID" : "⏳ UNPAID";
                string paymentDate = _invoice.IsPaid
                    ? _invoice.IssuedDate.AddDays(2).ToString("dd MMMM yyyy")
                    : "-";

                lblInvoiceInfo = CreateInfoLabel(yPos);
                lblInvoiceInfo.Text =
                    $"Invoice No      : INV-{DateTime.Now.Year}-{_invoice.Id:D4}\n" +
                    $"Invoice Date    : {_invoice.IssuedDate:dd MMMM yyyy}\n" +
                    $"Due Date        : {_invoice.DueDate:dd MMMM yyyy}\n" +
                    $"\n" +
                    $"─────────────────────────────────────────────\n" +
                    $"Berth Fee ({duration} days @ Rp {berthFeePerDay:N0})    : Rp {berthFeeTotal:N0}\n" +
                    $"Special Fee                         : Rp {specialFee:N0}\n" +
                    $"Service Fee                         : Rp {serviceFee:N0}\n" +
                    $"─────────────────────────────────────────────\n" +
                    $"Subtotal                            : Rp {subtotal:N0}\n" +
                    $"Tax (11%)                           : Rp {taxAmount:N0}\n" +
                    $"─────────────────────────────────────────────\n" +
                    $"TOTAL AMOUNT                        : Rp {totalAmount:N0}\n" +
                    $"\n" +
                    $"Payment Status  : {paymentStatus}\n" +
                    $"Payment Date    : {paymentDate}";
                lblInvoiceInfo.Height = 350;
                lblInvoiceInfo.Font = new Font("Consolas", 9);
                scrollContent.Controls.Add(lblInvoiceInfo);

                // Enable PDF button if invoice exists
                btnDownloadPdf.Visible = true;
            }
            else
            {
                // No invoice yet
                var lblNoInvoice = CreateInfoLabel(yPos);
                lblNoInvoice.Text = "Invoice belum tersedia. Invoice akan dibuat setelah request disetujui.";
                lblNoInvoice.ForeColor = Color.FromArgb(127, 140, 141);
                lblNoInvoice.Height = 40;
                scrollContent.Controls.Add(lblNoInvoice);

                btnDownloadPdf.Visible = false;
            }
        }

        private async void btnDownloadPdf_Click(object sender, EventArgs e)
        {
            if (_invoice == null || _ship == null || _berth == null || _assignment == null)
            {
                ShowError("Insufficient data to generate PDF!");
                return;
            }

            try
            {
                IsExporting = true;

                // Generate PDF in background
                await Task.Run(() =>
                {
                    string pdfPath = _pdfService.GenerateInvoicePdf(
                        _currentRequest!,
                        _ship,
                        _berth,
                        _assignment,
                        _invoice,
                        _currentUser
                    );

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
            this.ClientSize = new Size(700, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Text = "Booking Detail";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Panel
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 60;
            panelHeader.BackColor = Color.FromArgb(52, 73, 94);

            lblTitle = new Label();
            lblTitle.Text = "📋 DETAIL PEMESANAN DOCKING";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.AutoSize = true;

            panelHeader.Controls.Add(lblTitle);

            // Scrollable Content Panel
            scrollContent = new Panel();
            scrollContent.Location = new Point(20, 80);
            scrollContent.Size = new Size(660, 740);
            scrollContent.BackColor = Color.White;
            scrollContent.BorderStyle = BorderStyle.FixedSingle;
            scrollContent.AutoScroll = true;

            // Download PDF Button
            btnDownloadPdf = new Button();
            btnDownloadPdf.Text = "📄 Download Invoice PDF";
            btnDownloadPdf.Size = new Size(250, 45);
            btnDownloadPdf.Location = new Point(150, 830);
            btnDownloadPdf.BackColor = Color.FromArgb(33, 150, 243); // Blue
            btnDownloadPdf.ForeColor = Color.White;
            btnDownloadPdf.FlatStyle = FlatStyle.Flat;
            btnDownloadPdf.FlatAppearance.BorderSize = 0;
            btnDownloadPdf.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnDownloadPdf.Cursor = Cursors.Hand;
            btnDownloadPdf.Click += btnDownloadPdf_Click;
            btnDownloadPdf.Visible = false; // Hidden until invoice loaded

            // Close Button
            btnClose = new Button();
            btnClose.Text = "✕ Tutup";
            btnClose.Size = new Size(150, 45);
            btnClose.Location = new Point(410, 830);
            btnClose.BackColor = Color.FromArgb(158, 158, 158); // Gray
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += btnClose_Click;

            // Add controls to form
            this.Controls.Add(panelHeader);
            this.Controls.Add(scrollContent);
            this.Controls.Add(btnDownloadPdf);
            this.Controls.Add(btnClose);
        }

        private Label CreateSectionHeader(string text, int yPos)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(52, 73, 94);
            lbl.Location = new Point(15, yPos);
            lbl.Size = new Size(620, 25);
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
            lbl.Size = new Size(600, 60);
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
