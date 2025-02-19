namespace VinylShop.Models
{
    public class Genre
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty; //зробити унік ключ + у артисті

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}
