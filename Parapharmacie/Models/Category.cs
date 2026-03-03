using System.ComponentModel.DataAnnotations;

namespace Parapharmacie.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}




