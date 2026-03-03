using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; }

        // Optional link to customer or supplier actor when relevant
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
    }
}




