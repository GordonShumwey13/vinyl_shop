using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
        
        public string RoleName { get; set; }

    }
}
