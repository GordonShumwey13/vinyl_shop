using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class OrderController : Controller
    {
        private readonly VinylShopContext _context;

        public OrderController(VinylShopContext context)
        {
            _context = context;
        }

        // GET: Shop/Order/Create
        [HttpGet]
        public async Task<IActionResult> Create(int albumId)
        {
            var album = await _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null) return NotFound();

            ViewBag.Album = album;
            return View();
        }

        // POST: Shop/Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int albumId, string email)
        {
            var album = await _context.Albums.FindAsync(albumId);
            if (album == null) return NotFound();

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == email);
            if (buyer == null)
            {
                buyer = new Buyer { Email = email };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            var order = new Order
            {
                AlbumId = album.Id,
                Price = album.Price,
                BuyerId = buyer.Id,
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
