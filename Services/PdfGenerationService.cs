using HarborMaster.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;

namespace HarborMaster.Services
{
    /// <summary>
    /// Service for generating PDF documents for Ship Details and Invoices
    /// Uses QuestPDF library for PDF generation
    /// </summary>
    public class PdfGenerationService
    {
        public PdfGenerationService()
        {
            // Set QuestPDF license for Community use (free)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Generate Ship Detail PDF and save to user's Downloads folder
        /// </summary>
        /// <param name="ship">Ship object with all details</param>
        /// <returns>Full path to generated PDF file</returns>
        public string GenerateShipDetailPdf(Ship ship)
        {
            try
            {
                // Generate filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string safeShipName = GetSafeFilename(ship.Name);
                string filename = $"Ship_Detail_{safeShipName}_{timestamp}.pdf";

                // Save to Downloads folder
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );
                string fullPath = Path.Combine(downloadsPath, filename);

                // Generate PDF
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                        // Header
                        page.Header()
                            .Height(100)
                            .Background(Colors.Blue.Darken3)
                            .Padding(15)
                            .Column(column =>
                            {
                                column.Spacing(5);
                                column.Item().AlignCenter().Text("🏢 HARBOR MASTER SYSTEM")
                                    .FontSize(18).Bold().FontColor(Colors.White);
                                column.Item().AlignCenter().Text("Ship Information Report")
                                    .FontSize(13).FontColor(Colors.Grey.Lighten2);
                                column.Item().AlignCenter().Text($"Generated: {DateTime.Now:dd MMMM yyyy HH:mm}")
                                    .FontSize(9).FontColor(Colors.Grey.Lighten1);
                            });

                        // Content
                        page.Content()
                            .PaddingVertical(20)
                            .Column(column =>
                            {
                                column.Spacing(15);

                                // Ship Identity Section
                                column.Item().Element(container => SectionHeader(container, "📌 SHIP IDENTITY"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Spacing(3);
                                    col.Item().Text($"Ship Name         : {ship.Name}").FontSize(10);
                                    col.Item().Text($"IMO Number        : {ship.ImoNumber}").FontSize(10);
                                    col.Item().Text($"Ship Type         : {ship.ShipType}").FontSize(10);
                                });

                                // Specifications Section
                                column.Item().Element(container => SectionHeader(container, "📏 SPECIFICATIONS"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Spacing(3);
                                    col.Item().Text($"Length Overall    : {ship.LengthOverall:N2} meters").FontSize(10);
                                    col.Item().Text($"Draft             : {ship.Draft:N2} meters").FontSize(10);
                                });

                                // Fees & Priority Section
                                column.Item().Element(container => SectionHeader(container, "💰 FEES & PRIORITY"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Spacing(3);
                                    col.Item().Text($"Special Fee       : Rp {ship.CalculateSpecialFee():N0}").FontSize(10);
                                    col.Item().Text($"Priority Level    : {ship.GetPriorityLevel()}/5").FontSize(10);
                                    col.Item().Text($"Max Docking       : {ship.GetMaxDockingDuration()} days").FontSize(10);
                                });

                                // Required Services Section
                                column.Item().Element(container => SectionHeader(container, "✅ REQUIRED SERVICES"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Spacing(3);
                                    var services = ship.GetRequiredServices();
                                    foreach (var service in services)
                                    {
                                        col.Item().Text($"✓ {service}").FontSize(10);
                                    }
                                });
                            });

                        // Footer
                        page.Footer()
                            .Height(40)
                            .AlignCenter()
                            .Column(column =>
                            {
                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                column.Item().PaddingTop(10).Text("Harbor Master System © 2024")
                                    .FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                    });
                })
                .GeneratePdf(fullPath);

                return fullPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate Ship Detail PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Generate Invoice PDF for a docking request
        /// </summary>
        public string GenerateInvoicePdf(DockingRequest request, Ship ship, Berth berth, BerthAssignment assignment, Invoice invoice, User owner)
        {
            try
            {
                // Generate invoice number from ID
                string invoiceNumber = $"INV-{DateTime.Now.Year}-{invoice.Id:D4}";

                // Generate filename
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string safeInvoiceNo = GetSafeFilename(invoiceNumber);
                string safeShipName = GetSafeFilename(ship.Name);
                string filename = $"Invoice_{safeInvoiceNo}_{safeShipName}_{timestamp}.pdf";

                // Save to Downloads folder
                string downloadsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "Downloads"
                );
                string fullPath = Path.Combine(downloadsPath, filename);

                // Calculate duration in days
                var duration = (assignment.ETD - assignment.ETA).Days;
                var berthFeePerDay = 500000m; // Rp 500,000 per day
                var berthFeeTotal = berthFeePerDay * duration;
                var specialFee = ship.CalculateSpecialFee();
                var serviceFee = 750000m; // Fixed service fee
                var subtotal = berthFeeTotal + specialFee + serviceFee;
                var taxRate = 0.11m; // 11% tax
                var taxAmount = subtotal * taxRate;
                var totalAmount = subtotal + taxAmount;

                // Payment status
                string paymentStatus = invoice.IsPaid ? "PAID" : "UNPAID";
                string paymentDate = invoice.IsPaid ? invoice.IssuedDate.AddDays(2).ToString("dd MMMM yyyy") : "-";

                // Generate PDF
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                        // Header
                        page.Header()
                            .Height(120)
                            .Background(Colors.Green.Darken3)
                            .Padding(20)
                            .Column(column =>
                            {
                                column.Item().AlignCenter().Text("🏢 HARBOR MASTER SYSTEM")
                                    .FontSize(24).Bold().FontColor(Colors.White);
                                column.Item().AlignCenter().PaddingTop(10).Text("Port Authority Invoice")
                                    .FontSize(16).FontColor(Colors.Grey.Lighten2);
                            });

                        // Content
                        page.Content()
                            .PaddingVertical(20)
                            .Column(column =>
                            {
                                column.Spacing(15);

                                // Invoice Header Info
                                column.Item().Background(Colors.Grey.Lighten3).Padding(15).Column(col =>
                                {
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("INVOICE NO").Bold().FontSize(12);
                                        row.RelativeItem().AlignRight().Text(invoiceNumber).FontSize(12);
                                    });
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("DATE").Bold();
                                        row.RelativeItem().AlignRight().Text($"{invoice.IssuedDate:dd MMMM yyyy}");
                                    });
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text("DUE DATE").Bold();
                                        row.RelativeItem().AlignRight().Text($"{invoice.DueDate:dd MMMM yyyy}");
                                    });
                                });

                                // Bill To Section
                                column.Item().Element(container => SectionHeader(container, "BILL TO:"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Item().Text(owner.FullName).Bold();
                                    col.Item().Text($"Email: {owner.Email}");
                                    if (!string.IsNullOrEmpty(owner.Phone))
                                        col.Item().Text($"Phone: {owner.Phone}");
                                });

                                // Ship Details Section
                                column.Item().Element(container => SectionHeader(container, "SHIP DETAILS:"));
                                column.Item().PaddingLeft(20).Column(col =>
                                {
                                    col.Item().Text($"{ship.Name} ({ship.ImoNumber})").Bold();
                                    col.Item().Text($"Berth: {berth.BerthName} - {berth.Location}");
                                    col.Item().Text($"Period: {assignment.ETA:dd MMM yyyy} - {assignment.ETD:dd MMM yyyy} ({duration} days)");
                                });

                                // Itemized Charges
                                column.Item().Element(container => SectionHeader(container, "ITEMIZED CHARGES"));
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Description
                                        columns.RelativeColumn(1); // Qty
                                        columns.RelativeColumn(1); // Rate
                                        columns.RelativeColumn(1); // Amount
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Qty").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rate").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Amount").Bold();
                                    });

                                    // Berth Fee
                                    table.Cell().Padding(5).Text("Berth Fee");
                                    table.Cell().Padding(5).Text(duration.ToString());
                                    table.Cell().Padding(5).Text($"{berthFeePerDay:N0}");
                                    table.Cell().Padding(5).AlignRight().Text($"{berthFeeTotal:N0}");

                                    // Special Fee
                                    table.Cell().Padding(5).Text("Special Fee");
                                    table.Cell().Padding(5).Text("1");
                                    table.Cell().Padding(5).Text($"{specialFee:N0}");
                                    table.Cell().Padding(5).AlignRight().Text($"{specialFee:N0}");

                                    // Service Fee
                                    table.Cell().Padding(5).Text("Service Fee");
                                    table.Cell().Padding(5).Text("1");
                                    table.Cell().Padding(5).Text($"{serviceFee:N0}");
                                    table.Cell().Padding(5).AlignRight().Text($"{serviceFee:N0}");
                                });

                                // Totals
                                column.Item().PaddingTop(10).AlignRight().Column(col =>
                                {
                                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("Subtotal").Bold();
                                        row.RelativeItem().AlignRight().Text($"Rp {subtotal:N0}");
                                    });
                                    col.Item().Row(row =>
                                    {
                                        row.RelativeItem().Text($"Tax ({taxRate * 100}%)").Bold();
                                        row.RelativeItem().AlignRight().Text($"Rp {taxAmount:N0}");
                                    });
                                    col.Item().BorderTop(1).BorderColor(Colors.Grey.Medium).PaddingTop(5).Row(row =>
                                    {
                                        row.RelativeItem().Text("TOTAL AMOUNT").Bold().FontSize(13);
                                        row.RelativeItem().AlignRight().Text($"Rp {totalAmount:N0}").Bold().FontSize(13);
                                    });
                                });

                                // Payment Status
                                column.Item().PaddingTop(20).Background(invoice.IsPaid ? Colors.Green.Lighten4 : Colors.Orange.Lighten4).Padding(15).Column(col =>
                                {
                                    col.Item().Text($"PAYMENT STATUS: {paymentStatus}").Bold().FontSize(12);
                                    if (invoice.IsPaid)
                                    {
                                        col.Item().Text($"Payment Date: {paymentDate}");
                                        col.Item().Text("Payment Method: Bank Transfer");
                                    }
                                    else
                                    {
                                        col.Item().Text($"Please pay before: {invoice.DueDate:dd MMMM yyyy}");
                                    }
                                });
                            });

                        // Footer
                        page.Footer()
                            .Height(50)
                            .AlignCenter()
                            .Column(column =>
                            {
                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                                column.Item().PaddingTop(10).Text("Thank you for your business!")
                                    .FontSize(10).FontColor(Colors.Grey.Medium);
                                column.Item().PaddingTop(5).Text("Harbor Master System © 2024")
                                    .FontSize(9).FontColor(Colors.Grey.Medium);
                            });
                    });
                })
                .GeneratePdf(fullPath);

                return fullPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate Invoice PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to create section headers
        /// </summary>
        private static void SectionHeader(IContainer container, string title)
        {
            container.Padding(5)
                .Background(Colors.Blue.Lighten4)
                .Text(title)
                .Bold()
                .FontSize(12);
        }

        /// <summary>
        /// Get safe filename by removing invalid characters
        /// </summary>
        private static string GetSafeFilename(string filename)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", filename.Split(invalidChars));
        }

        /// <summary>
        /// Open PDF file with default application
        /// </summary>
        public static void OpenPdfFile(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to open PDF file: {ex.Message}", ex);
            }
        }
    }
}
