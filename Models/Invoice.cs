using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Postgrest.Attributes;
using Postgrest.Models;

namespace HarborMaster.Models
{
    [Table("invoices")]
    public class Invoice : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        [Column("berth_assignment_id")]
        public int BerthAssignmentId { get; set; }
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
        [Column("issued_date")]
        public DateTime IssuedDate { get; set; }
        [Column("due_date")]
        public DateTime DueDate { get; set; }
        [Column("is_paid")]
        public bool IsPaid { get; set; } = false;
        [Column("notes")]
        public string Notes { get; set; }
    }
}
