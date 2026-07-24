using QuickCommerce.Core.Enums;
using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Entities
{
    public class Store
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public string City { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Pincode { get; set; } = null!;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        // =========================
        // BUSINESS INFORMATION
        // =========================

        public string? BusinessName { get; set; }

        public string? GstNumber { get; set; }

        public string? PanNumber { get; set; }

        public string? LicenseNumber { get; set; }

        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }

        // =========================
        // ENTERPRISE STATUS CONTROL
        // =========================

        public StoreStatus Status { get; set; } = StoreStatus.Active;

        public bool IsVerified { get; set; } = false;

        // =========================
        // Legacy flags (temporary)
        // =========================

        public bool IsActive { get; set; } = true;
        public bool IsOnline { get; set; } = true;

        // =========================
        // Audit
        // =========================

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // =========================
        // Navigation
        // =========================

        public ICollection<StoreProduct>? StoreProducts { get; set; }
        public ICollection<UserStore>? UserStores { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}