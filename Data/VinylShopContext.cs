using Microsoft.EntityFrameworkCore;
using VinylShop.Models;

namespace VinylShop.Data
{
    public class VinylShopContext : DbContext
    {
        public VinylShopContext(DbContextOptions<VinylShopContext> options) : base(options) { }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
    }
}