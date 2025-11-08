using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Invoice
    {
        public string InvoiceID { get; set; }
        public string ShipID { get; set; }
        public decimal TotalAmount { get; private set; }
        public string PaymentStatus { get; private set; } // Unpaid, Paid
        public DateTime IssuedDate { get; private set; }

        private List<PortService> _services = new List<PortService>();
        private Ship _ship;

        public static Invoice GenerateInvoice(Ship ship)
        {
            return new Invoice
            {
                InvoiceID = Guid.NewGuid().ToString(),
                ShipID = ship.ShipID,
                _ship = ship,
                IssuedDate = DateTime.Now,
                PaymentStatus = "Unpaid"
            };
        }

        public void AddService(PortService service)
        {
            _services.Add(service);
        }

        // Memanfaatkan polymorphism dari PortService
        public decimal CalculateTotal()
        {
            TotalAmount = 0;
            foreach (var s in _services)
            {
                // Menggunakan polymorphic method
                if (_ship != null)
                {
                    TotalAmount += s.CalculateCost(_ship);
                }
                else
                {
                    TotalAmount += s.CalculateCost();
                }
            }
            return TotalAmount;
        }

        public void MarkAsPaid()
        {
            PaymentStatus = "Paid";
        }

        // Method baru untuk mendapatkan detail invoice
        public string GetInvoiceDetails()
        {
            StringBuilder details = new StringBuilder();
            details.AppendLine($"=== INVOICE {InvoiceID} ===");
            details.AppendLine($"Issued Date: {IssuedDate}");
            
            if (_ship != null)
            {
                details.AppendLine($"Ship: {_ship.Name} (ID: {_ship.ShipID})");
                details.AppendLine($"Ship Type: {_ship.Type}");
            }
            else
            {
                details.AppendLine($"Ship ID: {ShipID}");
            }
            
            details.AppendLine($"\nServices:");
            foreach (var service in _services)
            {
                decimal cost = _ship != null ? service.CalculateCost(_ship) : service.CalculateCost();
                details.AppendLine($"- {service.GetServiceDescription()}: ${cost:N2}");
            }
            
            details.AppendLine($"\nTotal Amount: ${TotalAmount:N2}");
            details.AppendLine($"Payment Status: {PaymentStatus}");
            
            return details.ToString();
        }

        public List<PortService> GetServices()
        {
            return new List<PortService>(_services);
        }
    }
}
