using System.ComponentModel.DataAnnotations;

namespace VinylShop.DbModel
{
    public class AlbumEditDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(255, ErrorMessage = "The title must be at most 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int ArtistId { get; set; }

        [Required]
        public int GenreId { get; set; }

        public string? ImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99.")]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        public List<SongDto> Songs { get; set; } = new();
    }

    public class SongDto
    {
        public int? Id { get; set; } 

        [Required(ErrorMessage = "The title is required.")]
        [StringLength(255, ErrorMessage = "The title must be at most 255 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required.")]
        public string Duration { get; set; } = "00:00:00";
    }
}
