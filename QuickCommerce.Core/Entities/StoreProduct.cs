using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("storeproducts")]
    public class StoreProduct
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("storeid")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;

        [Required]
        [Column("productid")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        // Pricing
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Column("mrp", TypeName = "decimal(10,2)")]
        public decimal? MRP { get; set; }

        // Inventory
        [Column("stockquantity")]
        public int StockQuantity { get; set; }

        [Column("lowstockthreshold")]
        public int LowStockThreshold { get; set; } = 5;

        // Availability
        [Column("isavailable")]
        public bool IsAvailable { get; set; } = true;

        // Audit
        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updatedat")]
        public DateTime? UpdatedAt { get; set; }
    }
}