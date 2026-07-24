using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Entities
{
    public class Cart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int StoreId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}