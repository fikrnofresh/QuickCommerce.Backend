using System;
using System.Collections.Generic;

namespace QuickCommerce.Core.Entities
{
    public class Store
    {
        public int Id { get; set; }

        // Relationship
        public int FranchiseId { get; set; }
        public Franchise Franchise { get; set; } = null!;

        // Basic Info
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!; // Unique short code

        // Location
        public string City { get; set; } = null!;
        public string Area { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Pincode { get; set; } = null!;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Contact
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        // Operational Timing
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsOnline { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation (we will connect later)
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<UserStore> UserStores { get; set; }
    }
}