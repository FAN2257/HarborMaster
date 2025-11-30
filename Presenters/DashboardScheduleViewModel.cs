using System;

namespace HarborMaster.Presenters
{
    public class DashboardScheduleViewModel
    {
        public string ShipName { get; set; }
        public string BerthName { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public string Status { get; set; }
        public DateTime? ActualArrivalTime { get; set; }
    }
}