namespace QuickCommerce.Core.DTOs.AdminRoles
{
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}