using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VinylShop.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly VinylShopContext _context;

        public AlbumsController(VinylShopContext context)
        {
            _context = context;
        }

        // List Albums with Sorting
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParam"] = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewData["ArtistSortParam"] = sortOrder == "artist_asc" ? "artist_desc" : "artist_asc";
            ViewData["GenreSortParam"] = sortOrder == "genre_asc" ? "genre_desc" : "genre_asc";

            var albums = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsQueryable();

            albums = ApplySorting(albums, sortOrder);
            return View(await albums.ToListAsync());
        }

        // View Album Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (album == null) return NotFound();

            return View(album);
        }

        // Create Album - GET
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
        }

        // Create Album - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? ImageFile, string ExistingArtist, string NewArtist, string GenreId, string NewGenre, List<string> SongTitles,
    List<string> SongDurations)
        {
            // Assign Artist
            var artist = await GetOrCreateArtist(ExistingArtist, NewArtist);
            if (artist == null)
            {
                ModelState.AddModelError("Artist", "Please select or enter an artist.");
                PopulateDropDowns();
                return View(album);
            }
            album.Artist = artist;

            // Assign Genre
            int genreId = await GetOrCreateGenre(GenreId, NewGenre);
            if (genreId == 0)
            {
                ModelState.AddModelError("Genre", "Please select or enter a genre.");
                PopulateDropDowns();
                return View(album);
            }
            album.GenreId = genreId;

            // Image Upload
            album.ImagePath = await UploadImageAsync(ImageFile);

            // Create Songs
            if (SongTitles != null && SongDurations != null)
            {
                for (int i = 0; i < SongTitles.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(SongTitles[i]) && !string.IsNullOrWhiteSpace(SongDurations[i]))
                    {
                        album.Songs.Add(new Song
                        {
                            Title = SongTitles[i],
                            Duration = TimeSpan.TryParse(SongDurations[i], out TimeSpan duration) ? duration : TimeSpan.Zero
                        });
                    }
                }
            }

            // Save Album
            _context.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete Album - GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            return album == null ? NotFound() : View(album);
        }

        // Delete Album - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Sorting Logic
        private IQueryable<Album> ApplySorting(IQueryable<Album> albums, string sortOrder)
        {
            return sortOrder switch
            {
                "title_desc" => albums.OrderByDescending(a => a.Title),
                "title_asc" => albums.OrderBy(a => a.Title),
                "artist_desc" => albums.OrderByDescending(a => a.Artist.Name),
                "artist_asc" => albums.OrderBy(a => a.Artist.Name),
                "genre_desc" => albums.OrderByDescending(a => a.Genre.Name),
                "genre_asc" => albums.OrderBy(a => a.Genre.Name),
                _ => albums.OrderBy(a => a.Title)
            };
        }

        // Dropdowns for Artists & Genres
        private void PopulateDropDowns()
        {
            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
        }

        // Get or Create Artist
        private async Task<Artist?> GetOrCreateArtist(string existingArtistId, string newArtistName)
        {
            if (!string.IsNullOrWhiteSpace(existingArtistId) && int.TryParse(existingArtistId, out int artistId))
            {
                return await _context.Artists.FindAsync(artistId);
            }
            else if (!string.IsNullOrWhiteSpace(newArtistName))
            {
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name.ToLower() == newArtistName.Trim().ToLower());
                if (artist == null)
                {
                    artist = new Artist { Name = newArtistName.Trim() };
                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();
                }
                return artist;
            }
            return null;
        }

        // Get or Create Genre
        private async Task<int> GetOrCreateGenre(string genreId, string newGenreName)
        {
            if (!string.IsNullOrWhiteSpace(genreId) && int.TryParse(genreId, out int parsedGenreId))
            {
                return parsedGenreId;
            }
            else if (!string.IsNullOrWhiteSpace(newGenreName))
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == newGenreName.Trim().ToLower());
                if (genre == null)
                {
                    genre = new Genre { Name = newGenreName.Trim() };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }
                return genre.Id;
            }
            return 0;
        }

        // Image Upload
        private async Task<string?> UploadImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/img/" + uniqueFileName;
        }
    }
}
