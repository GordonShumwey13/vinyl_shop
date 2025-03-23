using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Admin.Controllers
{   
    [Area("Admin")]
    public class ArtistsController : Controller
    {
        private readonly VinylShopContext _context;

        public ArtistsController(VinylShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var artists = await _context.Artists.ToListAsync();
            return View(artists);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var artist = await _context.Artists
                .Include(a => a.Albums)
                .ThenInclude(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (artist == null) return NotFound();

            return View(artist);
        }

        private async Task<string?> UploadImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/artists");
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/img/artists/" + uniqueFileName;
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> EditArtist(int? id)
        {
            if (id == null) return NotFound();

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            return View("EditArtist", artist);
        }

        // POST: Artists/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtist(int id, [Bind("Id,Name,Biography,ImagePath")] Artist artist, IFormFile? imageFile)
        {
            if (id != artist.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        artist.ImagePath = await UploadImageAsync(imageFile);
                    }
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View("EditArtis", artist);
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }

    }
}
