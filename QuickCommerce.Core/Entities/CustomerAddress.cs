using System;

namespace QuickCommerce.Core.Entities
{
    public class CustomerAddress
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Label { get; set; } // Home / Work

        public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Pincode { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}