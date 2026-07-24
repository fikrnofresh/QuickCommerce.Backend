using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("onboarding_requests")]
    public class OnboardingRequest
    {
        [Key]
        public int Id { get; set; }

        // =========================
        // TYPE & STATUS
        // =========================
        public string Type { get; set; } = string.Empty; // PLATFORM_USER / STORE_USER / DELIVERY / DELIVERY_PARTNER
        public string Status { get; set; } = "PENDING";

        // =========================
        // BASIC INFO
        // =========================
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Email { get; set; }

        // =========================
        // PROFILE
        // =========================
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }

        // =========================
        // ADDRESS
        // =========================
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // =========================
        // STORE LINK
        // =========================
        public int? StoreId { get; set; }

        // =========================
        // KYC
        // =========================
        public string? AadharNumber { get; set; }
        public string? PanNumber { get; set; }
        public string? DrivingLicenseNumber { get; set; }

        // =========================
        // DELIVERY DETAILS
        // =========================
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }

        // =========================
        // ACCESS REQUEST
        // =========================
        public string? RequestedRole { get; set; }

        public string? RequestedPermissions { get; set; }
        // JSON: ["ORDERS_VIEW","ORDERS_UPDATE"]

        // =========================
        // COMPLIANCE
        // =========================
        public bool IsKycVerified { get; set; } = false;

        public string? DocumentUrls { get; set; } // JSON

        // =========================
        // APPROVAL
        // =========================
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectionReason { get; set; }

        // =========================
        // METADATA (FUTURE PROOF)
        // =========================
        public string? Metadata { get; set; }

        // =========================
        // AUDIT
        // =========================
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}