using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public ICollection<UserRole> Roles { get; set; }
    }
}
