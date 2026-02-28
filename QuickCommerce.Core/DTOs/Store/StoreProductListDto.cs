namespace QuickCommerce.Core.DTOs.Store
{
    public class StoreProductListDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal? Price { get; set; }
        public decimal? MRP { get; set; }

        public string Unit { get; set; } = string.Empty;

        public int AvailableStock { get; set; }

        public bool IsLowStock { get; set; }
        public bool IsAvailable { get; set; }

        public string? CategoryName { get; set; }

        public string[]? ImageUrls { get; set; }
    }
}