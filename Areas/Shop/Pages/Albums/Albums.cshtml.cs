using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Albums
{
    public class AlbumsModel : PageModel
    {
        private readonly VinylShopContext _context;

        public AlbumsModel(VinylShopContext context)
        {
            _context = context;
        }

        public IList<Album> Albums { get; set; } = new List<Album>();

        [BindProperty(SupportsGet = true)]
        public string? Genre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sort { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsQueryable();

            switch (Sort)
            {
                case "latest":
                    query = query.OrderByDescending(a => a.Id);
                    break;
                case "artist":
                    query = query.OrderBy(a => a.Artist.Name);
                    break;
                case "genre":
                    query = query.OrderBy(a => a.Genre.Name);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(a => a.Price);
                    break;
                case "price_asc":
                    query = query.OrderBy(a => a.Price);
                    break;
                case "rating":
                    break;
                case null:
                case "":
                default:
                    break;
            }

            Albums = await query.ToListAsync();
        }
    }
}
