using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs.Commission
{
    public class CreateCategoryCommissionDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal CommissionPercent { get; set; }

        [Required]
        public decimal MinimumCommissionPerOrder { get; set; }
    }
}