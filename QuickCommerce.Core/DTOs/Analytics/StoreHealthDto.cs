namespace QuickCommerce.Core.DTOs.Analytics
{
    public class StoreHealthDto
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int Orders { get; set; }
        public decimal Revenue { get; set; }
        public int CancelledOrders { get; set; }
        public decimal CancellationRate { get; set; }
    }
}