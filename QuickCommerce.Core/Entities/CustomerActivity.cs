using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("customer_activity")]
    public class CustomerActivity
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        // VIEW, ADD_TO_CART, ORDER_PLACED, SEARCH
        [Required]
        [Column("action")]
        public string Action { get; set; } = string.Empty;

        // ProductId / CategoryId / OrderId
        [Column("entity_id")]
        public int? EntityId { get; set; }

        // JSON data (search text, filters, etc)
        [Column("metadata", TypeName = "jsonb")]
        public string? Metadata { get; set; }

        // Future AI scoring
        [Column("score")]
        public decimal? Score { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}