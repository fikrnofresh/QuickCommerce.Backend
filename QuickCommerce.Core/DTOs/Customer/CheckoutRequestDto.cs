namespace QuickCommerce.Core.DTOs.Customer
{
    public class CheckoutRequestDto
    {
        public int AddressId { get; set; }

        public string PaymentMethod { get; set; } = "COD";
    }
}