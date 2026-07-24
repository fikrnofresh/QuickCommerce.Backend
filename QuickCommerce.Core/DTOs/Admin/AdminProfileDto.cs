using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.DTOs.Admin
{
    public class AdminProfileDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string RoleScope { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public List<string> Permissions { get; set; } = new();

        public DateTime? LastLogin { get; set; }
    }
}