using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.ViewModels
{
    public class SupplierOrderItemInput
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class SupplierOrderCreateViewModel
    {
        [Required]
        public int SupplierId { get; set; }

        [MinLength(1, ErrorMessage = "Ajoutez au moins un produit.")]
        public List<SupplierOrderItemInput> Items { get; set; } = new();
    }
}




