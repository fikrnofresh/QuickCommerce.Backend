using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("orderstatushistory")]
    public class OrderStatusHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("orderid")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        // ✅ FIXED (correct mapping)
        [Column("old_status")]
        public string? OldStatus { get; set; }

        // ✅ FIXED
        [Required]
        [Column("new_status")]
        public string NewStatus { get; set; } = string.Empty;

        // ✅ FIXED
        [Column("changed_by_user_id")]
        public int? ChangedByUserId { get; set; }

        [Column("remarks")]
        public string? Remarks { get; set; }

        // ✅ FIXED
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}