using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        public decimal LineTotal => Math.Round(Quantity * UnitPrice * (1 - DiscountPercent / 100), 2);
    }
}




