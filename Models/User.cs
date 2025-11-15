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
        [PrimaryKey("id", true)]
        public int Id { get; set; }
        
        [Column("username")]
        public string Username { get; set; }
        
        [Column("full_name")]
        public string FullName { get; set; }
        
        [Column("role")]
        [JsonConverter(typeof(StringEnumConverter))]
        public UserRole Role { get; set; }
        
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        public bool CanOverrideAllocation()
        {
            return Role == UserRole.HarborMaster;
        }
    }
}
