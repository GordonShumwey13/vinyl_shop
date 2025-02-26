using Microsoft.EntityFrameworkCore;
using VinylShop.Models;

namespace VinylShop.Data
{
    public class VinylShopContext : DbContext
    {
        public VinylShopContext(DbContextOptions<VinylShopContext> options) : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Artist>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}
