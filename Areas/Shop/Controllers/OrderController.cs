using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Create(int albumId, string buyerEmail)
        {
            var album = await _context.Albums.FindAsync(albumId);
            if (album == null || album.Stock <= 0)
            {
                ModelState.AddModelError("", "This album is out of stock.");
                ViewBag.Albums = new SelectList(await _context.Albums.ToListAsync(), "Id", "Title", albumId);
                return View();
            }

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == buyerEmail);
            if (buyer == null)
            {
                buyer = new Buyer { Email = buyerEmail };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            album.Stock--;

            var order = new Order
            {
                AlbumId = albumId,
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
