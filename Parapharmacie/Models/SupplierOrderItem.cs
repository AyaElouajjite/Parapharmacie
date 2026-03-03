using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class SupplierOrderItem
    {
        public int Id { get; set; }

        public int SupplierOrderId { get; set; }
        public SupplierOrder? SupplierOrder { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int QuantityOrdered { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityReceived { get; set; }
    }
}




