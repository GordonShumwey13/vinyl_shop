using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(255, ErrorMessage = "The title must be at most 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist Artist { get; set; } = null!;

        [Required]
        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
