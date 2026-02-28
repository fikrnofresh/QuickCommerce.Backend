using System.ComponentModel.DataAnnotations;

namespace QuickCommerce.Core.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Unit { get; set; } = string.Empty;
    }
}
