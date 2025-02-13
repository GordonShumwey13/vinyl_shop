using Microsoft.AspNetCore.Mvc;
using VinylShop.Data;
using Microsoft.EntityFrameworkCore;

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
    }
}
