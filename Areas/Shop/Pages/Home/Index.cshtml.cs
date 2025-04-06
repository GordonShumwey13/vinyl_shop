using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly VinylShopContext _context;

        public IndexModel(VinylShopContext context)
        {
            _context = context;
        }

        public IList<Album> Albums { get; set; } = new List<Album>();

        public async Task OnGetAsync()
        {
            Albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Take(3)
                .ToListAsync();
        }
    }
}
