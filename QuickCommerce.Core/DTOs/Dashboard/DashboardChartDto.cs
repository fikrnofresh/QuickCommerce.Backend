using System;

namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class DashboardChartDto
    {
        public DateTime Date { get; set; }

        public int Orders { get; set; }

        public decimal Revenue { get; set; }
    }
}