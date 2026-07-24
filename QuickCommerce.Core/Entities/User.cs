using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Entities
{
    public class User
    {
        // =====================================================
        // PRIMARY KEY
        // =====================================================

        public int Id { get; set; }

        // =====================================================
        // IDENTITY
        // =====================================================

        public string? Username { get; set; }

        public string? FullName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        // =====================================================
        // PROFILE
        // =====================================================

        public string? ProfileImageUrl { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PreferredLanguage { get; set; } = "en";

        public string? TimeZone { get; set; } = "Asia/Kolkata";

        // =====================================================
        // EMPLOYMENT
        // =====================================================

        public string? EmployeeCode { get; set; }

        public string? Department { get; set; }

        public string? Designation { get; set; }

        public DateTime? JoiningDate { get; set; }

        // =====================================================
        // SECURITY
        // =====================================================

        public bool IsPhoneVerified { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsAdmin { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public bool IsLocked { get; set; } = false;

        public bool TwoFactorEnabled { get; set; } = false;

        public int FailedLoginAttempts { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public DateTime? PasswordChangedAt { get; set; }

        // =====================================================
        // AUDIT
        // =====================================================

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // =====================================================
        // NAVIGATION
        // =====================================================

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<UserStore> UserStores { get; set; } = new List<UserStore>();
    }
}