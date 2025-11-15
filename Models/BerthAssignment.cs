using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest.Models;
using Postgrest.Attributes;

namespace HarborMaster.Models
{
    [Table("berth_assignments")]
    public class BerthAssignment : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        [Column("ship_id")]
        public int ShipId { get; set; }
        [Column("berth_id")]
        public int BerthId { get; set; }
        [Column("eta")]
        public DateTime ETA { get; set; }
        [Column("etd")]
        public DateTime ETD { get; set; }
        [Column("actual_arrival_time")]
        public DateTime? ActualArrivalTime { get; set; }
        [Column("actual_departure_time")]
        public DateTime? ActualDepartureTime { get; set; }
        [Column("status")]
        public string Status { get; set; } // Scheduled, Arrived, Departed, Delayed
    }
}
