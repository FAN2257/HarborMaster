using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class Berth
    {
        public string BerthID { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string Status { get; private set; } = "Available";

        public bool CheckAvailability()
        {
            return Status == "Available";
        }

        public void AssignShip(Ship ship)
        {
            if (!CheckAvailability())
            {
                throw new InvalidOperationException("Berth is not available.");
            }

            Status = "Occupied";
            ship.Arrive();
        }

        public void ReleaseShip(Ship ship)
        {
            if (Status != "Occupied")
            {
                throw new InvalidOperationException("Berth is already available.");
            }

            Status = "Available";
            ship.Depart();
        }
    }
}
