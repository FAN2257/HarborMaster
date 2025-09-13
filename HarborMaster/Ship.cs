using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Ship
    {
        public string ShipID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Flag { get; set; }
        public DateTime? ArrivalTime { get; private set; }
        public DateTime? DepartureTime { get; private set; }
        public string Status { get; private set; } // Arrived, Departed, Waiting

        public void Arrive()
        {
            Status = "Arrived";
            ArrivalTime = DateTime.Now;
        }

        public void Depart()
        {
            Status = "Departed";
            DepartureTime = DateTime.Now;
        }

        public void UpdateStatus(string newStatus)
        {
            Status = newStatus;
        }
    }
}
