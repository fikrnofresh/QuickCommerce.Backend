using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Customer
{
    public class CustomerOrderDetailDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<CustomerOrderItemDto> Items { get; set; }
    }

    public class CustomerOrderItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal TotalPrice { get; set; }
    }
}