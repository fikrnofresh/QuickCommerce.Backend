using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("adminsessions")]
    public class AdminSession
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userid")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("refreshtoken")]
        public string RefreshToken { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("deviceinfo")]
        public string? DeviceInfo { get; set; }

        [MaxLength(200)]
        [Column("browser")]
        public string? Browser { get; set; }

        [MaxLength(200)]
        [Column("operatingsystem")]
        public string? OperatingSystem { get; set; }

        [MaxLength(50)]
        [Column("ipaddress")]
        public string? IpAddress { get; set; }

        [Column("logintime")]
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;

        [Column("lastactivity")]
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;

        [Column("logouttime")]
        public DateTime? LogoutTime { get; set; }

        [Column("iscurrent")]
        public bool IsCurrent { get; set; } = true;

        [Column("isrevoked")]
        public bool IsRevoked { get; set; }

        [MaxLength(300)]
        [Column("revokereason")]
        public string? RevokeReason { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}