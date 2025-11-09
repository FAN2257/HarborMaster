using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Invoice // Changed from internal to public
    {
        public int invoiceID { get; set; }
        public string shipID { get; set; }
        public decimal totalAmount { get; set; }
        public string paymentStatus { get; set; }
        public DateTime issuedDate { get; set; }

        // Add property to store services (updated for new service types)
        private List<PortServiceBase> _services = new List<PortServiceBase>();
        private Ship? _ship; // Store ship reference for cost calculation

        public void generateInvoice(string shipID, decimal amount)
        {
            this.shipID = shipID;
            this.totalAmount = amount;
            this.paymentStatus = "Unpaid";
            this.issuedDate = DateTime.Now;
            // Logic to save invoice to database or send to accounting system
        }

        public PortService[] addService(PortService[] services)
        {
            // Legacy method - keep for backward compatibility
            return services;
        }

        // Add method to support new service types
        public void AddService(PortServiceBase service)
        {
            _services.Add(service);
            RecalculateTotal();
        }

        // Overload for legacy PortService
        public void AddService(PortService service)
        {
            // Handle legacy PortService - convert or handle differently
            // For now, just recalculate based on ship
            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            if (_ship != null)
            {
                // Start with ship's base docking fee
                totalAmount = _ship.CalculateDockingFee();

                // Add service costs
                foreach (var service in _services)
                {
                    totalAmount += service.CalculateCost(_ship);
                }
            }
        }

        public decimal calculateTotal()
        {
            return totalAmount;
        }

        // Add method to support demo functionality
        public decimal CalculateTotal()
        {
            return calculateTotal();
        }

        public void markAsPaid()
        {
            this.paymentStatus = "Paid";
            // Logic to update invoice status in database
        }

        // Add static method to support demo functionality
        public static Invoice GenerateInvoice(Ship ship)
        {
            var invoice = new Invoice
            {
                invoiceID = new Random().Next(1000, 9999),
                shipID = ship.ShipID,
                totalAmount = ship.CalculateDockingFee(), // Use the ship's docking fee as base
                paymentStatus = "Unpaid",
                issuedDate = DateTime.Now,
                _ship = ship // Store ship reference
            };

            return invoice;
        }

        // Add method to get invoice details
        public string GetInvoiceDetails()
        {
            var details = new StringBuilder();
            details.AppendLine($"Invoice ID: {invoiceID}");
            details.AppendLine($"Ship ID: {shipID}");
            details.AppendLine($"Issue Date: {issuedDate:dd/MM/yyyy}");
            details.AppendLine($"Payment Status: {paymentStatus}");

            if (_ship != null)
            {
                details.AppendLine($"Ship: {_ship.Name} ({_ship.Type})");
                details.AppendLine($"Base Docking Fee: ${_ship.CalculateDockingFee():N2}");
            }

            if (_services.Any())
            {
                details.AppendLine("\nServices:");
                foreach (var service in _services)
                {
                    decimal cost = _ship != null ? service.CalculateCost(_ship) : 0;
                    details.AppendLine($"- {service.GetServiceDescription()}: ${cost:N2}");
                }
            }

            details.AppendLine($"\nTotal Amount: ${totalAmount:N2}");
            return details.ToString();
        }

        // Add InvoiceID property for compatibility (using the existing invoiceID field)
        public string InvoiceID => invoiceID.ToString();
    }
}
