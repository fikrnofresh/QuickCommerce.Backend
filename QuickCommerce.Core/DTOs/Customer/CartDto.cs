using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Customer
{
    public class CartDto
    {
        public int Id { get; set; }

        public int StoreId { get; set; }

        public List<CartItemDto> Items { get; set; } = new();

        public decimal TotalAmount { get; set; }
    }
}