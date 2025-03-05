using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(255, ErrorMessage = "The title must be at most 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int AlbumId { get; set; }

        public TimeSpan Duration { get; set; } = TimeSpan.Zero;

        public Album Album { get; set; } = null!;
    }
}
