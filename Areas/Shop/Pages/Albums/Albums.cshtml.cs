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
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Genre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Sort { get; set; }

        public string? CurrentSort => Sort;
        public string? CurrentGenre => Genre;

        public async Task OnGetAsync()
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(a => 
                    a.Title.Contains(SearchString) || 
                    a.Artist.Name.Contains(SearchString));
            }

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
                case null:
                case "":
                default:
                    query = query.OrderByDescending(a => a.Id);
                    break;
            }

            Albums = await query.ToListAsync();
        }

        public async Task<IActionResult> OnGetSearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new JsonResult(new List<object>());
            }

            var results = await _context.Albums
                .Include(a => a.Artist)
                .Where(a => a.Title.Contains(query) || a.Artist.Name.Contains(query))
                .Select(a => new {
                    id = a.Id,
                    title = a.Title,
                    artist = a.Artist.Name,
                    image = a.ImagePath
                })
                .Take(5)
                .ToListAsync();

            return new JsonResult(results);
        }
    }
}
