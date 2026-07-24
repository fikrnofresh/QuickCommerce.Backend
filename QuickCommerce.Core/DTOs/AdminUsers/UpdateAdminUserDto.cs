using System;
using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs.AdminUsers
{
    public class UpdateAdminUserDto
    {
        // =========================
        // Personal Information
        // =========================

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? UserName { get; set; }

        [StringLength(20)]
        public string? EmployeeCode { get; set; }

        // =========================
        // Organization
        // =========================

        public int RoleId { get; set; }

        public int? StoreId { get; set; }

        // =========================
        // Status
        // =========================

        public bool IsActive { get; set; }

        // =========================
        // Audit
        // =========================

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}