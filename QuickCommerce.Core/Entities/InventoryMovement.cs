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
        // STORE
        // =========================

        [Required]
        [Column("store_id")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;

        // =========================
        // STORE PRODUCT LINK
        // (important for multi-store inventory)
        // =========================

        [Required]
        [Column("store_product_id")]
        public int StoreProductId { get; set; }

        [ForeignKey("StoreProductId")]
        public StoreProduct StoreProduct { get; set; } = null!;

        // =========================
        // STOCK CHANGE
        // =========================

        [Required]
        [Column("quantity_changed")]
        public int QuantityChanged { get; set; }

        // =========================
        // MOVEMENT TYPE
        // =========================

        [Required]
        [MaxLength(50)]
        [Column("movement_type")]
        public string MovementType { get; set; } = "ADJUSTMENT";
        /*
            ADJUSTMENT
            ORDER_DEDUCT
            ORDER_CANCEL_RETURN
            STOCK_ADD
        */

        // =========================
        // REASON
        // =========================

        [MaxLength(255)]
        [Column("reason")]
        public string? Reason { get; set; }

        // =========================
        // USER AUDIT
        // =========================

        [Column("performed_by_user_id")]
        public int? PerformedByUserId { get; set; }

        // =========================
        // CREATED TIME
        // =========================

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}