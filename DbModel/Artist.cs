using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required.")]
        [StringLength(255, ErrorMessage = "The name must be at most 255 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}

