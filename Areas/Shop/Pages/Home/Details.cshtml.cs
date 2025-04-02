using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Pages.Shop.Home
{
    public class DetailsModel : PageModel
    {
        private readonly VinylShopContext _context;

        public DetailsModel(VinylShopContext context)
        {
            _context = context;
        }

        public Album Album { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Album == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
