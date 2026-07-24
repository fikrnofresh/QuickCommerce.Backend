public class BundleRecommendationDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }

    public int StoreProductId { get; set; }
    public decimal Price { get; set; }
    public string Unit { get; set; }
    public string ImageUrl { get; set; }

    public int Frequency { get; set; }
}