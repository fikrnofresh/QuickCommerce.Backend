namespace QuickCommerce.Core.DTOs.Customer
{
    public class RecommendationDto
    {
        public int ProductId { get; set; }
        public string Type { get; set; } = string.Empty; // VIEWED / BOUGHT / SUGGESTED
        public int Score { get; set; }
    }
}