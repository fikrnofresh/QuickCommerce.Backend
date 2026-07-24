using System;

namespace QuickCommerce.Core.DTOs.Customer
{
    public class CustomerOrderTrackingDto
    {
        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}