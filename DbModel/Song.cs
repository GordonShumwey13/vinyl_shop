using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Song
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty; // має бути маст, змінити з емпті

        public int AlbumId { get; set; }

        public TimeSpan Duration { get; set; }

        public Album Album { get; set; } = null!;
    }
}
