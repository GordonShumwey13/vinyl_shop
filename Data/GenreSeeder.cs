using VinylShop.Models;
using VinylShop.Data;

namespace VinylShop.SeedData
{
    public static class GenreSeeder
    {
        public static void Seed(VinylShopContext context)
        {
            if (!context.Genres.Any())
            {
                var genres = new List<Genre>
                {
                    new Genre { Name = "Rock" },
                    new Genre { Name = "Jazz" },
                    new Genre { Name = "Hip-Hop" },
                    new Genre { Name = "Indie Rock" },
                    new Genre { Name = "Alternative Rock" },
                    new Genre { Name = "Pop" },
                    new Genre { Name = "Electronic" },
                    new Genre { Name = "Metal" },
                    new Genre { Name = "Folk" },
                    new Genre { Name = "Blues" },
                    new Genre { Name = "Other" }
                };

                context.Genres.AddRange(genres);
                context.SaveChanges();
            }
        }
    }
}
