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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;database=vinyl_shop;user=root;password=qwerty123",
                    ServerVersion.AutoDetect("server=localhost;database=vinyl_shop;user=root;password=qwerty123")
                );
            }
        }
    }
}
