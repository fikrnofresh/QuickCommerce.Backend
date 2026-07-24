namespace QuickCommerce.Core.DTOs.Analytics
{
    public class RestockAlertDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int StoreId { get; set; }
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
    }
}