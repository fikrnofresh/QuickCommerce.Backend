using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public BusinessHealthDto BusinessHealth { get; set; }

        public GrowthMetricsDto GrowthMetrics { get; set; }

        public FinancialInsightsDto FinancialInsights { get; set; }

        public OrderInsightsDto OrderInsights { get; set; }

        public StoreInsightsDto StoreInsights { get; set; }

        public List<TopStoreDto> TopStores { get; set; }

        public List<TopProductDto> TopProducts { get; set; }

        public List<TopProductDto> LowSellingProducts { get; set; }

        public List<InventoryAlertDto> LowStockProducts { get; set; }

        public List<InventoryAlertDto> OutOfStockProducts { get; set; }

        public List<CategoryPerformanceDto> TopCategories { get; set; }

        public List<CategoryPerformanceDto> SlowCategories { get; set; }

        public List<string> Alerts { get; set; }
    }

    public class BusinessHealthDto
    {
        public int OrdersToday { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal PlatformCommissionToday { get; set; }
        public int ActiveStores { get; set; }
        public int OrdersInProgress { get; set; }
        public int CancelledOrders { get; set; }
    }

    public class GrowthMetricsDto
    {
        public decimal OrdersGrowthPercent { get; set; }
        public decimal RevenueGrowthPercent { get; set; }
        public int WeeklyOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class FinancialInsightsDto
    {
        public decimal PlatformCommissionMonth { get; set; }
        public decimal StorePayoutToday { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class OrderInsightsDto
    {
        public int PendingOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int OutForDelivery { get; set; }
        public int DelayedOrders { get; set; }
    }

    public class StoreInsightsDto
    {
        public int OfflineStores { get; set; }
        public int InactiveStores { get; set; }
    }
}