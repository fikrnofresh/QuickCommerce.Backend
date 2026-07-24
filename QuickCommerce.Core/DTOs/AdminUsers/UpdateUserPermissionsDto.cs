namespace QuickCommerce.Core.DTOs.AdminUsers
{
    public class UpdateUserPermissionsDto
    {
        public int RoleId { get; set; }

        public string PermissionsJson { get; set; } = string.Empty;
    }
}