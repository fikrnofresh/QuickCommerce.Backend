using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Scope { get; set; } = "STORE"; // PLATFORM or STORE

        public int? StoreId { get; set; }

        public bool IsSystemRole { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}