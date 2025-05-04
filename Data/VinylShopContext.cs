using Microsoft.EntityFrameworkCore;
using VinylShop.Models;

namespace VinylShop.Data
{
    public class VinylShopContext : DbContext
    {
        public VinylShopContext(DbContextOptions<VinylShopContext> options) : base(options) { }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<UserAdmin> UserAdmins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artist>()
                .HasIndex(a => a.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .ToTable("User")
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserAdmin>()
                .ToTable("UserAdmin")
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
                .ToTable("UserRole");

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.UserAdmin)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserAdminId);
        }
    }
}
