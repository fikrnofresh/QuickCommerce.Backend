namespace QuickCommerce.Core.DTOs.Store
{
    public class StoreDashboardDto
    {
        public int StoreId { get; set; }

        public decimal RevenueToday { get; set; }

        public int OrdersToday { get; set; }

        public int OrdersInProgress { get; set; }

        public int PendingOrders { get; set; }

        public int PreparingOrders { get; set; }

        public int ReadyForPickupOrders { get; set; }

        public int LowStockProducts { get; set; }

        public int OutOfStockProducts { get; set; }
    }
}