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
        public async Task<IActionResult> OrderForm(int albumId, int quantity = 1)
        {
            var album = await _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null) return NotFound();

            ViewBag.Album = album;
            ViewBag.Quantity = quantity;
            return View("Create");
        }

        // POST: Shop/Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int albumId, string buyerEmail, int quantity = 1)
        {
            var album = await _context.Albums.FindAsync(albumId);
            if (album == null || album.Stock < quantity)
            {
                ModelState.AddModelError("", "This album is out of stock.");
                ViewBag.Album = album;
                ViewBag.Quantity = quantity;
                return View();
            }

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == buyerEmail);
            if (buyer == null)
            {
                buyer = new Buyer { Email = buyerEmail };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            album.Stock -= quantity;

            // Create multiple orders or just one with quantity? Choose your approach:
            for (int i = 0; i < quantity; i++)
            {
                _context.Orders.Add(new Order
                {
                    AlbumId = albumId,
                    Price = album.Price,
                    BuyerId = buyer.Id,
                    OrderDate = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        // GET: Shop/Order/Success
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
