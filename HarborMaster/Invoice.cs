using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    internal class Invoice
    {
        public string InvoiceID { get; set; }
        public string ShipID { get; set; }
        public decimal TotalAmount { get; private set; }
        public string PaymentStatus { get; private set; } // Unpaid, Paid
        public DateTime IssuedDate { get; private set; }

        private List<PortService> _services = new List<PortService>();

        public static Invoice GenerateInvoice(Ship ship)
        {
            return new Invoice
            {
                InvoiceID = Guid.NewGuid().ToString(),
                ShipID = ship.ShipID,
                IssuedDate = DateTime.Now,
                PaymentStatus = "Unpaid"
            };
        }

        public void AddService(PortService service)
        {
            _services.Add(service);
        }

        public decimal CalculateTotal()
        {
            TotalAmount = 0;
            foreach (var s in _services)
            {
                TotalAmount += s.CalculateCost();
            }
            return TotalAmount;
        }

        public void MarkAsPaid()
        {
            PaymentStatus = "Paid";
        }
    }
}
