using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Buyer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [StringLength(255, ErrorMessage = "The email must be at most 255 characters.")]
        public string Email { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
