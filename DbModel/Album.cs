namespace VinylShop.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public int ArtistId { get; set; }
        public Artist? Artist { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        
        public string? ImagePath { get; set; }
    }
}
