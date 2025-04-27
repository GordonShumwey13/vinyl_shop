using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    public class Buyer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The email is required.")]
        [StringLength(255, ErrorMessage = "The email must be at most 255 characters.")]
        public string Email { get; set; } = string.Empty;

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
