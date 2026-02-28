namespace QuickCommerce.Core.DTOs.Admin
{
    public class DashboardSummaryDto
    {
        public int OrdersToday { get; set; }

        public decimal RevenueToday { get; set; }

        public decimal AverageOrderValueToday { get; set; }

        public double CancelRateToday { get; set; }

        public int PendingOrders { get; set; }

        public int LowStockProductsCount { get; set; }

        public int ActiveCustomersToday { get; set; }
    }
}