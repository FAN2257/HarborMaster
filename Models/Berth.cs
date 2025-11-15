using System;
using Postgrest.Attributes;
using Postgrest.Models;

namespace HarborMaster.Models
{
    [Table("berths")]
    public class Berth : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("berth_name")]
        public string BerthName { get; set; }
        
        [Column("location")]
        public string Location { get; set; }
        
        [Column("max_length")]
        public double MaxLength { get; set; }
        
        [Column("max_draft")]
        public double MaxDraft { get; set; }
        
        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("base_rate_per_day")]
        public decimal BaseRatePerDay { get; set; }
    }
}
