namespace QuickCommerce.Core.DTOs.StoreUsers
{
    public class StoreUserResponseDto
    {
        public int UserId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}