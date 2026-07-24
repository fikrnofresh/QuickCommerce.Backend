namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class TopStoreDto
    {
        public int StoreId { get; set; }

        public string StoreName { get; set; }
        
        public int Orders { get; set; }

        public decimal Revenue { get; set; }
    }
}