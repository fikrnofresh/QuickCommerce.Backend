using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("activity_logs")]
    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        // =========================
        // WHO PERFORMED
        // =========================
        public int? UserId { get; set; }  // existing (kept)

        public string? UserRole { get; set; }  // NEW

        // =========================
        // ACTION INFO
        // =========================
        public string Module { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        // =========================
        // TARGET ENTITY
        // =========================
        public string? EntityType { get; set; }  // renamed from EntityName (better)

        public int? EntityId { get; set; }

        // =========================
        // CHANGE TRACKING
        // =========================
        public string? Description { get; set; } // human readable

        public string? OldData { get; set; } // JSON

        public string? NewData { get; set; } // JSON

        public string? Metadata { get; set; } // existing (kept)

        // =========================
        // SYSTEM INFO
        // =========================
        public string? IpAddress { get; set; }

        public string? Device { get; set; } // NEW

        public string? Source { get; set; } // WEB / APP / ADMIN

        // =========================
        // TIMESTAMP
        // =========================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}