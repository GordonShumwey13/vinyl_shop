using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class UserAdmin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
    }

}
