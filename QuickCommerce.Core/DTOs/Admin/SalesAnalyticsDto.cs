using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class SalesAnalyticsDto
    {
        public SalesSummaryDto Summary { get; set; } = new();
        public List<SalesTrendDto> Trend { get; set; } = new();
        public SalesComparisonDto? Comparison { get; set; }
        public List<PeakHourDto> PeakHours { get; set; } = new();
    }

    public class SalesSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double GrowthPercentage { get; set; }
    }

    public class SalesTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Orders { get; set; }
    }

    public class SalesComparisonDto
    {
        public decimal PreviousRevenue { get; set; }
        public int PreviousOrders { get; set; }
    }

    public class PeakHourDto
    {
        public int Hour { get; set; }
        public int Orders { get; set; }
    }
}