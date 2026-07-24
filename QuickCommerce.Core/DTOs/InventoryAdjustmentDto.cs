namespace QuickCommerce.Core.DTOs
{
    public class InventoryAdjustmentDto
    {
        public int StoreProductId { get; set; }

        public int QuantityChange { get; set; }

        public string Reason { get; set; } = "";
    }
}