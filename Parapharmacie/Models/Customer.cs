using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Range(0, int.MaxValue)]
        public int LoyaltyPoints { get; set; }
    }
}




