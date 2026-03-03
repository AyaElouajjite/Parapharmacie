using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class SupplierOrder
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReceivedDate { get; set; }

        [Required]
        public string Status { get; set; } = "En attente";

        public List<SupplierOrderItem> Items { get; set; } = new();
    }
}




