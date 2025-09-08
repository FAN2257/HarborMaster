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
        public string Role { get; set; }

        public void CreateAssignment(Berth berth, Ship ship)
        {
            berth.AssignShip(ship);
        }

        public void MonitorTraffic()
        {
            //Logic Assignment
        }

        public void GenerateReport()
        {
            //Logic Assignment
        }
    }
}
