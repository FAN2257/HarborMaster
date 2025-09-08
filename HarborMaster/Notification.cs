using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    internal class Notification
    {
        public int notificationID { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }

        public void SendNotification(string message, string type)
        {
            this.message = message;
            this.type = type;
            this.timestamp = DateTime.Now;
            // Logic to send notification (e.g., email, SMS, in-app)
        }

        public void markAsRead()
        {
            // Logic to mark notification as read
        }
    }
}
