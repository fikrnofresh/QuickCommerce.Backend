namespace QuickCommerce.Core.DTOs.Store
{
    public class StoreInventoryMonitorDto
    {
        public int StoreId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}