using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("refreshtokens")]
    public class RefreshToken
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
        public string Token { get; set; }

        [Required]
        [Column("expiresat")]
        public DateTime ExpiresAt { get; set; }

        [Column("isrevoked")]
        public bool IsRevoked { get; set; } = false;

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("revokedat")]
        public DateTime? RevokedAt { get; set; }

        [Column("replacedbytoken")]
        public string? ReplacedByToken { get; set; }

        // Navigation
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}