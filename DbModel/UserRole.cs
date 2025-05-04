using System.ComponentModel.DataAnnotations;
using VinylShop.Enums;

namespace VinylShop.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserAdminId { get; set; }

        public UserAdmin UserAdmin { get; set; }

        public UserRoleEnum RoleName { get; set; }

    }
}
