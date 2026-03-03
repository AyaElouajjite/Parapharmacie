using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Reference { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

