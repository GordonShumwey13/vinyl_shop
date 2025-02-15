using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int AlbumId { get; set; }

        public TimeSpan Duration { get; set; }

        public Album Album { get; set; } = null!;
    }
}
