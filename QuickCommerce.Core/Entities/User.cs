using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{  
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        
        // 🔐 New fields
        public bool IsPhoneVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ActivityLog> ActivityLogs { get; set; }
        public ICollection<UserStore> UserStores { get; set; }
    }
}
