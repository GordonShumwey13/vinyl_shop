using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Buyer
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
