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

        public decimal TotalPrice { get; set; }

        public async Task<IActionResult> OnGetAsync(int albumId, int quantity = 1)
        {
            Album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == albumId);

            if (Album == null) return NotFound();

            Quantity = quantity;
            TotalPrice = Album.Price * Quantity;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int albumId)
        {
            Album = await _context.Albums.FindAsync(albumId);
            if (Album == null || Album.Stock < Quantity)
            {
                ModelState.AddModelError(string.Empty, "Insufficient stock.");
                return Page();
            }

            Album.Stock -= Quantity;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Orders/Success");
        }
    }
}
