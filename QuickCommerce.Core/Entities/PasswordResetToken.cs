using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("passwordresettokens")]
    public class PasswordResetToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userid")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [Column("expiresat")]
        public DateTime ExpiresAt { get; set; }

        [Required]
        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("usedat")]
        public DateTime? UsedAt { get; set; }

        [Required]
        [Column("isused")]
        public bool IsUsed { get; set; } = false;

        [Column("ipaddress")]
        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [Column("useragent")]
        [MaxLength(300)]
        public string? UserAgent { get; set; }

        [Column("createdby")]
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [Column("purpose")]
        [MaxLength(50)]
        public string Purpose { get; set; } = "PASSWORD_RESET";

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}