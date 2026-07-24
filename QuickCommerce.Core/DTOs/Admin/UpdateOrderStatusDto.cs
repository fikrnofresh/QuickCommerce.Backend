namespace QuickCommerce.Core.DTOs.Admin
{
    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}