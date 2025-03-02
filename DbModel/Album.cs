using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist? Artist { get; set; }

        public int GenreId { get; set; }

        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }

        public string? ImagePath { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
