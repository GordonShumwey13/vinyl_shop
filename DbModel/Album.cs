namespace VinylShop.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ArtistId { get; set; }
        public int GenreId { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string Url { get; set; } = null!;

        public Artist Artist { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
