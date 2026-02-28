using System;

namespace QuickCommerce.Core.DTOs
{
    public class DeliveryDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int PartnerId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
