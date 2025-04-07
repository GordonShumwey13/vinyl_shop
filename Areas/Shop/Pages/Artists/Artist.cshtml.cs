using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Artists
{
    public class ArtistModel : PageModel
    {
        private readonly VinylShopContext _context;

        public ArtistModel(VinylShopContext context)
        {
            _context = context;
        }

        public Artist? Artist { get; set; } = null;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Artist = await _context.Artists
                .Include(a => a.Albums)
                    .ThenInclude(album => album.Genre)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Artist == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
