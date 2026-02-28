using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class ProductAnalyticsDto
    {
        public List<ProductAnalyticsItemDto> Products { get; set; } = new();
    }

    public class ProductAnalyticsItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}