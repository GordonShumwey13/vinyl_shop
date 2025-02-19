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

        //Read
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .ToListAsync();
            return View(albums);
        }

        //Cereate
        public IActionResult Create()
        {
            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? ImageFile, string ArtistName)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Name == ArtistName);
            if (artist == null)
            {
                artist = new Artist { Name = ArtistName };
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            album.Artist = artist;

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
            else
            {
                album.ImagePath = null;
            }

            _context.Add(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete
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
