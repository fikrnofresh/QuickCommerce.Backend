using System;

namespace QuickCommerce.Core.DTOs.AdminUsers
{
    public class AdminUserResponseDto
    {
        // =========================
        // Identity
        // =========================

        public int Id { get; set; }

        public int? StoreId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string? EmployeeCode { get; set; }

        // =========================
        // Security
        // =========================

        /// <summary>
        /// Returned ONLY immediately after user creation.
        /// Never populate this in list/details APIs.
        /// </summary>
        public string? TemporaryPassword { get; set; }

        public int RoleId { get; set; }

        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public bool RequirePasswordChange { get; set; }

        // =========================
        // Audit
        // =========================

        public DateTime CreatedOn { get; set; }

        public DateTime? LastLoginOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        // =========================
        // Display
        // =========================

        public string? StoreName { get; set; }

        public string? CreatedBy { get; set; }
    }
}