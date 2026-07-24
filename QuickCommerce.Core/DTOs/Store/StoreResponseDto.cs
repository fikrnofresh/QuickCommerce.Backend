using QuickCommerce.Core.Enums;
using System;

namespace QuickCommerce.Core.DTOs.Store
{
    /// <summary>
    /// Store information returned to clients.
    /// </summary>
    public class StoreResponseDto
    {
        // =====================================================
        // IDENTIFICATION
        // =====================================================

        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        // =====================================================
        // LOCATION
        // =====================================================

        public string City { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        // =====================================================
        // CONTACT
        // =====================================================

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        // =====================================================
        // BUSINESS
        // =====================================================

        public string? BusinessName { get; set; }

        public string? GstNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? LicenseNumber { get; set; }

        // =====================================================
        // OPERATIONS
        // =====================================================

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        // =====================================================
        // STATUS
        // =====================================================

        public StoreStatus Status { get; set; }

        public bool IsVerified { get; set; }

        public bool IsActive { get; set; }

        public bool IsOnline { get; set; }

        // =====================================================
        // AUDIT
        // =====================================================

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}