using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("orderitems")]
    public class OrderItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("orderid")]
        public int OrderId { get; set; }

        [Required]
        [Column("productid")]
        public int ProductId { get; set; }

        [Required]
        [Column("productname")]
        public string ProductName { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("unit")]
        public string Unit { get; set; }

        [Required]
        [Column("priceperunit", TypeName = "decimal(10,2)")]
        public decimal PricePerUnit { get; set; }

        [Required]
        [Column("totalprice", TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
