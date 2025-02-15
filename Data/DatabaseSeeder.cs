using VinylShop.Models;
using VinylShop.Data;

namespace VinylShop.SeedData
{
    public static class DatabaseSeeder
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
                    new Genre { Name = "Alternative Rock" }
                };

                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            if (!context.Albums.Any())
            {
                Dictionary<string, Artist> artistDict = new();
                List<Album> albums = new();

                var rockGenre = context.Genres.First(g => g.Name == "Rock");
                var jazzGenre = context.Genres.First(g => g.Name == "Jazz");
                var hipHopGenre = context.Genres.First(g => g.Name == "Hip-Hop");
                var indieRockGenre = context.Genres.First(g => g.Name == "Indie Rock");
                var alternativeRockGenre = context.Genres.First(g => g.Name == "Alternative Rock");

                var vinylProducts = new List<(string artist, string album, string image, string url, Genre genre)>
                {
                    ("Waits, Tom", "Mule Variations", "img/1.jpg", "https://thevinyl.com.ua/uk/mule-variations", rockGenre),
                    ("Cullum, Jamie", "Momentum", "img/2.jpg", "https://thevinyl.com.ua/uk/momentum", jazzGenre),
                    ("KRS One", "Return of the Boom Bap", "img/3.jpg", "https://thevinyl.com.ua/uk/return-of-the-boom-bap", hipHopGenre),
                    ("Franz Ferdinand", "Tonight: Franz Ferdinand", "img/4.jpg", "https://thevinyl.com.ua/uk/tonight:-franz-ferdinand", indieRockGenre),
                    ("Arcade Fire", "Everything Now (Night Version)", "img/5.jpg", "https://thevinyl.com.ua/uk/everything-now-(night-version)", alternativeRockGenre),
                    ("Pretty Reckless", "Going To Hell (Coloured)", "img/6.jpg", "https://thevinyl.com.ua/uk/going-to-hell-(coloured)", rockGenre),
                    ("White Stripes", "Get Behind Me Satan", "img/7.jpg", "https://thevinyl.com.ua/uk/get-behind-me-satan", rockGenre),
                    ("Van Halen", "Fair Warning", "img/8.jpg", "https://thevinyl.com.ua/uk/fair-warning", rockGenre)
                };

                foreach (var (artistName, albumTitle, image, url, genre) in vinylProducts)
                {
                    if (!artistDict.ContainsKey(artistName))
                    {
                        artistDict[artistName] = new Artist { Name = artistName };
                    }

                    albums.Add(new Album
                    {
                        Title = albumTitle,
                        Artist = artistDict[artistName],
                        Genre = genre,
                        ImageUrl = image,
                        Url = url
                    });
                }

                context.Artists.AddRange(artistDict.Values);
                context.Albums.AddRange(albums);
                context.SaveChanges();
            }
        }
    }
}