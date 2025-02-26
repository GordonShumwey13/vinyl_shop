using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}
