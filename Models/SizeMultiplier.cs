using Postgrest.Attributes;
using Postgrest.Models;

namespace HarborMaster.Models
{
    [Table("size_multipliers")]
    public class SizeMultiplier : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("min_length")]
        public decimal MinLength { get; set; }

        [Column("max_length")]
        public decimal MaxLength { get; set; }

        [Column("multiplier")]
        public decimal Multiplier { get; set; }

        [Column("category")]
        public string Category { get; set; } = string.Empty;
    }
}
