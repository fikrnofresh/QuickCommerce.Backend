namespace QuickCommerce.Core.DTOs.Dashboard
{
    public class InventoryAlertDto
    {
        public int StoreId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int StockQuantity { get; set; }
    }
}