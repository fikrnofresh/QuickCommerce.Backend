using System;

namespace QuickCommerce.Core.Entities
{
    public class CategoryCommissionRule
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public decimal CommissionPercent { get; set; }

        public decimal MinimumCommissionPerOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}