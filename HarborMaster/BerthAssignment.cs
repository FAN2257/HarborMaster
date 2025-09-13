using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class BerthAssignment
    {
        public string AssignmentID { get; set; }
        public DateTime ArrivalTime { get; private set; }
        public DateTime DepartureTime { get; private set; }
        public string Status { get; private set; } // Scheduled, Completed

        public Ship AssignedShip { get; private set; }
        public Berth AssignedBerth { get; private set; }

        public void Schedule(Ship ship, Berth berth)
        {
            if (!berth.CheckAvailability())
            {
                throw new InvalidOperationException("Berth is not available for scheduling.");
            }

            AssignedShip = ship;
            AssignedBerth = berth;
            Status = "Scheduled";

            // langsung assign ke berth
            berth.AssignShip(ship);
            ArrivalTime = DateTime.Now;
        }

        public void CompleteAssignment()
        {
            if (Status != "Scheduled")
            {
                throw new InvalidOperationException("Assignment cannot be completed in current state.");
            }

            Status = "Completed";
            AssignedBerth.ReleaseShip(AssignedShip);
            DepartureTime = DateTime.Now;
        }

        public string GetAssignmentInfo()
        {
            return $"Assignment {AssignmentID}: Ship {AssignedShip.Name} at berth {AssignedBerth.BerthID} [{Status}]";
        }
    }
}
