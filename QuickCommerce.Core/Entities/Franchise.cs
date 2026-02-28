using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;

namespace QuickCommerce.Core.Entities
{
    public class Franchise
    {
        public int Id { get; set; }

        // Basic Info
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!; // Unique short code

        public string OwnerName { get; set; } = null!;
        public string OwnerPhone { get; set; } = null!;
        public string? OwnerEmail { get; set; }

        // Business Compliance
        public string? GSTNumber { get; set; }
        public string? PANNumber { get; set; }

        // Agreement
        public DateTime? AgreementStartDate { get; set; }
        public DateTime? AgreementEndDate { get; set; }

        // Status
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Store>? Stores { get; set; }
    }
}