using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickCommerce.Core.Entities
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("categoryid")]
        public int CategoryId { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column("mrp", TypeName = "decimal(10,2)")]
        public decimal? MRP { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("unit")]
        public string Unit { get; set; }

       
        [Required]
        [Column("isavailable")]
        public bool IsAvailable { get; set; }

        [Column("imageurls")]
        public string[]? ImageUrls { get; set; }

        [Column("searchkeywords")]
        public string? SearchKeywords { get; set; }

        // Navigation (DOCUMENT APPROVED)
        public Category? Category { get; set; }

    }
}
