using System;

namespace QuickCommerce.Core.DTOs
{
    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
