namespace QuickCommerce.Core.DTOs.StoreUsers
{
    public class CreateStoreUserDto
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public int RoleId { get; set; }
    }
}