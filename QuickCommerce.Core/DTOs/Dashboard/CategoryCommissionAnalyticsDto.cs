namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class CategoryCommissionAnalyticsDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal Revenue { get; set; }

        public decimal PlatformCommission { get; set; }

        public int Orders { get; set; }
    }
}