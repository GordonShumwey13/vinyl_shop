using VinylShop.Data;

namespace VinylShop.SeedData
{
    public static class DatabaseSeeder
    {
        public static void Seed(VinylShopContext context)
        {
            GenreSeeder.Seed(context);
        }
    }
}
