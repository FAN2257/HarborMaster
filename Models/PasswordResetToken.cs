using System;
using Postgrest.Models;
using Postgrest.Attributes;

namespace HarborMaster.Models
{
    /// <summary>
    /// Model untuk menyimpan token reset password
    /// Token memiliki masa berlaku terbatas (30 menit)
    /// </summary>
    [Table("password_reset_tokens")]
    public class PasswordResetToken : BaseModel
    {
        [PrimaryKey("id", shouldInsert: false)]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        /// <summary>
        /// Cek apakah token masih valid (belum expired dan belum digunakan)
        /// </summary>
        public bool IsValid()
        {
            return !IsUsed && DateTime.UtcNow < ExpiresAt;
        }
    }
}
