using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.ViewModels
{
    public class SaleItemInput
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }
    }

    public class SaleCreateViewModel
    {
        public int? CustomerId { get; set; }

        [MinLength(1, ErrorMessage = "Ajoutez au moins une ligne produit.")]
        public List<SaleItemInput> Items { get; set; } = new();
    }
}




