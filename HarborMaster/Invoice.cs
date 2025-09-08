using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    internal class Invoice
    {
        public int invoiceID { get; set; }
        public string shipID { get; set; }
        public decimal totalAmount { get; set; }
        public string paymentStatus { get; set; }
        public DateTime issuedDate { get; set; }

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
            // Logic to add services to the invoice
            return services;
        }

        public decimal calculateTotal()
        {
            return totalAmount;
        }

        public void markAsPaid()
        {
            this.paymentStatus = "Paid";
            // Logic to update invoice status in database
        }

    }
}
