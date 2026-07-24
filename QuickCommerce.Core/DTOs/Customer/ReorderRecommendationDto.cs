using System;

public class ReorderRecommendationDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public int StoreProductId { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; }
    public string ImageUrl { get; set; }

    public int OrderCount { get; set; }
    public DateTime LastOrderedAt { get; set; }
    public int Score { get; set; }
}