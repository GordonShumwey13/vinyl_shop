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

        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .ToListAsync();
            return View(albums);
        }

        public IActionResult Create()
        {
            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album)
        {
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Artists = new SelectList(_context.Artists, "Id", "Name");
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name");

            return View(album);
        }
    }
}
