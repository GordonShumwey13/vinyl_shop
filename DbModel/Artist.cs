using System.ComponentModel.DataAnnotations;

namespace VinylShop.Models
{
    public class Artist
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}