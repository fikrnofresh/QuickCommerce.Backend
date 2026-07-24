using System;

namespace QuickCommerce.Core.DTOs.Store
{
    public class StoreDto
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
        
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsOnline { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}