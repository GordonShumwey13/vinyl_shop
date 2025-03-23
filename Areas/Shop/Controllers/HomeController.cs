using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;

namespace VinylShop.Areas.Shop.Controllers
{   
    [Area("Shop")]
    public class HomeController : Controller
    {
        private readonly VinylShopContext _context;

        public HomeController(VinylShopContext context)
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
