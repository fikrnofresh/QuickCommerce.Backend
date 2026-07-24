using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class AdminLoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Role { get; set; } = string.Empty;
        
        public string RoleScope { get; set; } = string.Empty;

        public int? StoreId { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}