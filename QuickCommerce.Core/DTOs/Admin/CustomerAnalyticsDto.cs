using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class CustomerAnalyticsDto
    {
        public int TotalCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public double RepeatRatePercentage { get; set; }

        public List<CustomerSegmentDto> VipCustomers { get; set; } = new();
        public List<CustomerSegmentDto> ActiveCustomers { get; set; } = new();
        public List<CustomerSegmentDto> AtRiskCustomers { get; set; } = new();
        public List<CustomerSegmentDto> DeadCustomers { get; set; } = new();

        public List<TopCustomerDto> TopCustomers { get; set; } = new();
    }

    public class CustomerSegmentDto
    {
        public int CustomerId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
    }
}