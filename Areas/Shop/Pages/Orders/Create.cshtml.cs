using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly VinylShopContext _context;

        public CreateModel(VinylShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Album Album { get; set; } = null!;

        [BindProperty]
        public int Quantity { get; set; } = 1;

        [BindProperty]
        public string BuyerEmail { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int albumId, int quantity = 1)
        {
            Album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == albumId);

            if (Album == null)
            {
                return NotFound();
            }

            Quantity = quantity;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var album = await _context.Albums.FindAsync(Album.Id);

            if (album == null || album.Stock < Quantity)
            {
                ModelState.AddModelError(string.Empty, "Not enough stock available.");
                return Page();
            }

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == BuyerEmail);
            if (buyer == null)
            {
                buyer = new Buyer { Email = BuyerEmail };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            album.Stock -= Quantity;

            for (int i = 0; i < Quantity; i++)
            {
                var order = new Order
                {
                    AlbumId = album.Id,
                    BuyerId = buyer.Id,
                    Price = album.Price,
                    OrderDate = DateTime.UtcNow
                };
                _context.Orders.Add(order);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Success");
        }
    }
}
