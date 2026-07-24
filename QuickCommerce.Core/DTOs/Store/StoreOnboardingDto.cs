using System;
using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs.Store
{
    /// <summary>
    /// Enterprise Store Onboarding Request
    ///
    /// Creates:
    /// • Store
    /// • Store Administrator
    /// • Assign STORE_ADMIN Role
    /// • UserStore Mapping
    /// • Temporary Credentials
    /// </summary>
    public class StoreOnboardingDto
    {
        // =====================================================
        // STORE INFORMATION
        // =====================================================

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional.
        /// If empty system generates automatically.
        /// Example : STR00001
        /// </summary>
        [MaxLength(30)]
        public string? Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Area { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Pincode { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [Phone]
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        // =====================================================
        // BUSINESS INFORMATION
        // =====================================================

        [MaxLength(150)]
        public string? BusinessName { get; set; }

        [MaxLength(15)]
        public string? GstNumber { get; set; }

        [MaxLength(10)]
        public string? PanNumber { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        // =====================================================
        // STORE ADMIN
        // =====================================================

        [Required]
        [MaxLength(150)]
        public string OwnerName { get; set; } = string.Empty;

        [Phone]
        [Required]
        [MaxLength(15)]
        public string OwnerMobile { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        [MaxLength(150)]
        public string OwnerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Optional.
        /// If empty OwnerMobile becomes username.
        /// </summary>
        [MaxLength(100)]
        public string? Username { get; set; }

        // =====================================================
        // ADDRESS
        // =====================================================

        [MaxLength(250)]
        public string? AddressLine1 { get; set; }

        [MaxLength(250)]
        public string? AddressLine2 { get; set; }

        [MaxLength(100)]
        public string? Landmark { get; set; }

        // =====================================================
        // OPTIONS
        // =====================================================

        public bool AutoGeneratePassword { get; set; } = true;
        
        public string? TemporaryPassword { get; set; }

        public bool SendEmail { get; set; } = true;

        public bool SendSms { get; set; } = true;

        public bool SendWelcomeNotification { get; set; } = true;

        // =====================================================
        // AUDIT
        // =====================================================

        /// <summary>
        /// Logged-in Platform Admin Id.
        /// </summary>
        public int? CreatedBy { get; set; }
    }
}