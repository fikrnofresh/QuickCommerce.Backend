using System;

namespace QuickCommerce.Core.DTOs.Customer
{
    public class UpdateCustomerProfileDto
    {
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PreferredLanguage { get; set; }
    }
}