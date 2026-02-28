using System;

namespace QuickCommerce.Core.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    }
}
