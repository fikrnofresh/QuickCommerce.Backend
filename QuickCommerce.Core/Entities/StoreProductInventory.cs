using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("store_product_inventory")]
    public class StoreProductInventory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // =========================
        // STORE RELATION
        // =========================

        [Required]
        [Column("storeid")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;

        // =========================
        // PRODUCT RELATION
        // =========================

        [Required]
        [Column("productid")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        // =========================
        // STOCK INFO
        // =========================

        [Required]
        [Column("stock")]
        public int Stock { get; set; }

        [Column("lowstockthreshold")]
        public int LowStockThreshold { get; set; } = 5;

        // =========================
        // AUDIT
        // =========================

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updatedat")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}