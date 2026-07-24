namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class CategoryPerformanceDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public decimal Revenue { get; set; }

        public int Orders { get; set; }
    }
}