using System;
using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.Entities
{
    public class OtpVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public string OtpHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsVerified { get; set; } = false;

        public int AttemptCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
