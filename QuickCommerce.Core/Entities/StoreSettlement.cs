using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("store_settlements")]
    public class StoreSettlement
    {
        [Key]
        public int Id { get; set; }

        [Column("storeid")]
        public int StoreId { get; set; }

        [Column("totalorders")]
        public int TotalOrders { get; set; }

        [Column("totalrevenue", TypeName = "decimal(12,2)")]
        public decimal TotalRevenue { get; set; }

        [Column("platformcommission", TypeName = "decimal(12,2)")]
        public decimal PlatformCommission { get; set; }

        [Column("storepayout", TypeName = "decimal(12,2)")]
        public decimal StorePayout { get; set; }

        [Column("settlementdate")]
        public DateTime SettlementDate { get; set; }

        [Column("status")]
        public string Status { get; set; } = "PENDING";

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}