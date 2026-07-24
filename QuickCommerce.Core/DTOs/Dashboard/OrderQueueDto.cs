using System;

namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class OrderQueueDto
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        public int StoreId { get; set; }

        public string Status { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}