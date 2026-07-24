namespace QuickCommerce.Core.DTOs.Analytics
{
    public class ReorderRecommendationDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CurrentStock { get; set; }
        public int SoldLast7Days { get; set; }
        public int RecommendedReorder { get; set; }
    }
}