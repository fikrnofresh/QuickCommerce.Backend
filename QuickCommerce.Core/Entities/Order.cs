using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("ordernumber")]
        public string OrderNumber { get; set; }

        [Required]
        [Column("customerid")]
        public int CustomerId { get; set; }

        // =========================
        // 🏬 STORE SCOPE (NEW)
        // =========================

        [Required]
        [Column("storeid")]
        public int StoreId { get; set; }

        [ForeignKey("StoreId")]
        public Store Store { get; set; } = null!;

        // =========================
        // DELIVERY ADDRESS
        // =========================

        [Required]
        [Column("deliveryaddressid")]
        public int DeliveryAddressId { get; set; }

        [ForeignKey("DeliveryAddressId")]
        public Address? DeliveryAddress { get; set; }

        // =========================
        // STATUS
        // =========================

        [Required]
        [Column("status")]
        public string Status { get; set; } = "PENDING";

        // =========================
        // PRICING
        // =========================

        [Required]
        [Column("subtotalamount", TypeName = "decimal(10,2)")]
        public decimal SubtotalAmount { get; set; }

        [Column("deliveryfee", TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; } = 0;

        [Column("discountamount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Column("taxamount", TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Required]
        [Column("totalamount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        // =========================
        // PAYMENT
        // =========================

        [Required]
        [Column("paymentmode")]
        public string PaymentMode { get; set; }

        [Required]
        [Column("paymentstatus")]
        public string PaymentStatus { get; set; } = "PENDING";

        // =========================
        // NOTES
        // =========================

        [Column("deliveryinstructions")]
        public string? DeliveryInstructions { get; set; }

        // =========================
        // AUDIT
        // =========================

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updatedat")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // =========================
        // NAVIGATION
        // =========================

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}