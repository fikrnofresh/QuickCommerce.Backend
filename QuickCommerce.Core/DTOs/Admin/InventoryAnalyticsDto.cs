using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class InventoryAnalyticsDto
    {
        public List<LowStockDto> LowStockProducts { get; set; } = new();
        public List<FastMovingDto> FastMovingProducts { get; set; } = new();
        public decimal TotalInventoryValue { get; set; }
        public double StockTurnoverRatio { get; set; }
    }

    public class LowStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
    }

    public class FastMovingDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
    }
}