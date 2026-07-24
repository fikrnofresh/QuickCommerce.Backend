namespace QuickCommerce.Core.DTOs.AdminPermissions
{
    public class PermissionResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}