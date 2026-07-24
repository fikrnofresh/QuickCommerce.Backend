namespace QuickCommerce.Core.DTOs.Commission
{
    public class CategoryCommissionDto
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public decimal CommissionPercent { get; set; }

        public decimal MinimumCommissionPerOrder { get; set; }

        public bool IsActive { get; set; }
    }
}