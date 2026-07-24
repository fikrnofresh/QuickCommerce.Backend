namespace QuickCommerce.Core.DTOs.Customer
{
    public class AddCartItemDto
    {
        public int StoreId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}