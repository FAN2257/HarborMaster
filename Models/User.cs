using System;
using Postgrest.Models;
using Postgrest.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HarborMaster.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        // ? FIX: Set shouldInsert = false untuk auto-increment field
        [PrimaryKey("id", shouldInsert: false)]
        public int Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("role")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserRole Role { get; set; }

        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("company_name")]
        public string? CompanyName { get; set; }

        // Computed property for UI display - extracts first name
        // IMPORTANT: JsonIgnore prevents Supabase from trying to update this non-existent column
        [JsonIgnore]
        public string FirstName => FullName?.Split(' ').FirstOrDefault() ?? "User";

        public bool CanOverrideAllocation()
        {
            return Role == UserRole.HarborMaster;
        }
    }
}
