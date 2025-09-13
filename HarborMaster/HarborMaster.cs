using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarborMaster
{
    public class HarborMaster
    {
        public string HarborMasterID { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Role { get; set; } // Admin, Controller

        public BerthAssignment CreateAssignment(Ship ship, Berth berth, DateTime arrival, DateTime departure)
        {
            var assignment = new BerthAssignment
            {
                AssignmentID = Guid.NewGuid().ToString()
            };

            assignment.Schedule(ship, berth);
            return assignment;
        }

        public void MonitorTraffic(List<Ship> ships)
        {
            foreach (var s in ships)
            {
                Console.WriteLine($"{s.Name} is currently {s.Status}");
            }
        }

        public string GenerateReport(List<BerthAssignment> assignments)
        {
            string report = "=== Harbor Traffic Report ===\n";
            foreach (var a in assignments)
            {
                report += a.GetAssignmentInfo() + "\n";
            }
            return report;
        }
    }
}
