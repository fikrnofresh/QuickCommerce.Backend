using System;

namespace QuickCommerce.Core.DTOs.Customer
{
    public class CustomerOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}