using System;
using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs.Store
{
    /// <summary>
    /// Create Store Request
    /// </summary>
    public class CreateStoreDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

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
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? BusinessName { get; set; }

        public string? GstNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? LicenseNumber { get; set; }

        public TimeSpan? OpeningTime { get; set; }

        public TimeSpan? ClosingTime { get; set; }

        // Required by existing StoreService
        public bool IsActive { get; set; } = true;

        public bool IsOnline { get; set; } = true;
    }
}