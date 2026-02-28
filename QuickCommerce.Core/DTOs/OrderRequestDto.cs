using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs
{
    public class OrderRequestDto
    {
        public int CustomerId { get; set; }

        // 🏬 NEW – Store Scope
        public int StoreId { get; set; }

        public int DeliveryAddressId { get; set; }

        public string PaymentMode { get; set; } = string.Empty;
        public string? DeliveryInstructions { get; set; }

        public List<OrderItemRequestDto> Items { get; set; } = new();
    }
}