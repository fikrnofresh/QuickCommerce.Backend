using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("inventory_movements")]
    public class InventoryMovement
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // =========================
        // PRODUCT
        // =========================

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        // =========================
        // 🏬 STORE (NEW)
        // =========================

        [Required]
        [Column("storeid")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;

        // =========================
        // STOCK CHANGE
        // =========================

        [Required]
        [Column("quantity_changed")]
        public int QuantityChanged { get; set; }

        [Required]
        [Column("reason")]
        public string Reason { get; set; } = null!;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}