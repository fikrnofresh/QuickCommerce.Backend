using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public decimal SubtotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;

        public string? DeliveryInstructions { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new();
    }
}
