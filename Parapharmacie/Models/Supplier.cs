using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}




