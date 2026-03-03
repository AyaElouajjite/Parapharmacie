using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public List<SaleItem> Items { get; set; } = new();

        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }
    }
}




