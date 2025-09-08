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
        public DateTime AssignmentDate { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; }

        public Ship Ship { get; set; }
        public Berth Berth { get; set; }
        public Schedule Schedule { get; set; }
        public HarborMaster HarborMaster { get; set; }
        public List<PortService> PortService { get; set; } = new List<PortService>();

        public void Schedule(Ship ship, Berth berth, DateTime arrival, DateTime departure)
        {
            Ship = ship;
            Berth = berth;
            ArrivalTime = arrival;
            DepartureTime = departure;
            Status = "Scheduled";
        }

        public void CompleteAssignment()
        {
            Status = "Completed";
            Berth.ReleaseShip(Ship);
        }
        public GetAssignmentInfo()
        {
            return $"AssignmentID: {AssignmentID}, Ship: {Ship.Name}, Berth: {Berth.BerthID}, Arrival: {ArrivalTime}, Departure: {DepartureTime}, Status: {Status}";
        }
    }
}
