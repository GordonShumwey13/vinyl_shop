using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Albums
{
    public class DetailsModel : PageModel
    {
        private readonly VinylShopContext _context;

        public DetailsModel(VinylShopContext context)
        {
            _context = context;
        }

        public Album? Album { get; set; }
        public List<Review> Reviews { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Album = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Songs)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Album == null)
            {
                return NotFound();
            }

            Reviews = await _context.Reviews
                .Where(r => r.AlbumId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int AlbumId, string AuthorName, int Rating, string? Comment)
        {
            var album = await _context.Albums.Include(a => a.Reviews).FirstOrDefaultAsync(a => a.Id == AlbumId);
            if (album == null) return NotFound();

            var review = new Review
            {
                AlbumId = AlbumId,
                AuthorName = AuthorName,
                Rating = Rating,
                Comment = Comment
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Оновлення середнього рейтингу
            album.Rating = album.Reviews.Any() ? album.Reviews.Average(r => r.Rating) : 0;
            album.ReviewCount = album.Reviews.Count;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = AlbumId });
        }

    }
}
