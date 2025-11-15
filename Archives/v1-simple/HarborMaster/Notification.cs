using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    internal class Notification
    {
        public string NotificationID { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } // Arrival, Assignment, Invoice
        public string Status { get; private set; } // Unread, Read
        public DateTime Timestamp { get; private set; }

        public void SendNotification(string target)
        {
            Timestamp = DateTime.Now;
            Status = "Unread";
            Console.WriteLine($"Notification sent to {target}: {Message}");
        }

        public void MarkAsRead()
        {
            Status = "Read";
        }
    }
}
