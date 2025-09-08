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
        public string Capacity { get; set; }
        public string Status { get; set; }

        public bool CheckAvailability()
        {
            return Status == "Available";
        }

        public void AssignShip(Ship ship)
        {
            if (CheckAvailability())
            {
                Status = "Occupied";
                ship.Arrive();
            }
            else
            {
                throw new InvalidOperationException("Berth is not available.");
            }
        }

        public void ReleaseShip(Ship ship)
        {
            if (Status == "Occupied")
            {
                Status = "Available";
                ship.Depart();
            }
            else
            {
                throw new InvalidOperationException("Berth is already available.");
            }
        }
    }
}
