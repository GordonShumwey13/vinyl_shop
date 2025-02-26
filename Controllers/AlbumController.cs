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
        private readonly ILogger<AlbumsController> _logger;

        public AlbumsController(VinylShopContext context, ILogger<AlbumsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Read
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .ToListAsync();
            return View(albums);
        }

        // Create
        public IActionResult Create()
        {
            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? ImageFile, string ExistingArtist, string NewArtist, string GenreId, string NewGenre)
        {
            _logger.LogInformation("Attempting to create album: {AlbumTitle}", album.Title);

            // Обробка артиста
            Artist artist = null;
            if (!string.IsNullOrWhiteSpace(ExistingArtist) && int.TryParse(ExistingArtist, out int artistId))
            {
                artist = await _context.Artists.FindAsync(artistId);
            }
            else if (!string.IsNullOrWhiteSpace(NewArtist))
            {
                artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name.ToLower() == NewArtist.Trim().ToLower());
                if (artist == null)
                {
                    artist = new Artist { Name = NewArtist.Trim() };
                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("New artist '{ArtistName}' added.", artist.Name);
                }
            }

            if (artist == null)
            {
                ModelState.AddModelError("Artist", "Please select or enter an artist.");
                ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
                return View(album);
            }

            album.Artist = artist;

            // Обробка жанру
            if (!string.IsNullOrWhiteSpace(GenreId) && int.TryParse(GenreId, out int genreId))
            {
                album.GenreId = genreId;
            }
            else if (!string.IsNullOrWhiteSpace(NewGenre))
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == NewGenre.Trim().ToLower());
                if (genre == null)
                {
                    genre = new Genre { Name = NewGenre.Trim() };
                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("New genre '{GenreName}' added.", genre.Name);
                }
                album.GenreId = genre.Id;
            }

            if (album.GenreId == 0)
            {
                ModelState.AddModelError("Genre", "Please select or enter a genre.");
                ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
                return View(album);
            }

            // Обробка зображення
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                album.ImagePath = "/img/" + uniqueFileName;
            }

            // Збереження альбому
            try
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Album '{AlbumTitle}' created successfully.", album.Title);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving album '{AlbumTitle}'.", album.Title);
                ModelState.AddModelError(string.Empty, "An error occurred while saving the album. Please try again.");
                ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
                ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
                return View(album);
            }
        }

        // Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Albums == null)
            {
                return Problem("Album was not found.");
            }

            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
