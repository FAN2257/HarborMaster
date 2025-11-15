using Postgrest.Attributes;
using Postgrest.Models;

namespace HarborMaster.Models
{
    [Table("ship_type_multipliers")]
    public class ShipTypeMultiplier : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("ship_type")]
        public string ShipType { get; set; } = string.Empty;

        [Column("multiplier")]
        public decimal Multiplier { get; set; }

        [Column("description")]
        public string? Description { get; set; }
    }
}
