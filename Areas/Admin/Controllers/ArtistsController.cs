using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Enums;
using VinylShop.Models;
using VinylShop.Utils;

namespace VinylShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
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

        [Authorize(Roles = $"{nameof(UserRoleEnum.Admin)},{nameof(UserRoleEnum.SalesManager)}")]
        public async Task<IActionResult> EditArtist(int? id)
        {
            if (id == null) return NotFound();

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();

            return View("EditArtist", artist);
        }

        [Authorize(Roles = $"{nameof(UserRoleEnum.Admin)},{nameof(UserRoleEnum.SalesManager)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtist(int id, [Bind("Id,Name,Biography,ImagePath")] Artist artist, IFormFile? imageFile)
        {
            if (id != artist.Id) return NotFound();

            var imageError = ImageUpload.Validate(imageFile);
            if (imageError != null)
            {
                ModelState.AddModelError(string.Empty, imageError);
            }

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

            return View("EditArtist", artist);
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.Id == id);
        }

        private static Task<string?> UploadImageAsync(IFormFile? imageFile)
        {
            return ImageUpload.SaveAsync(imageFile, "artists");
        }
    }
}
