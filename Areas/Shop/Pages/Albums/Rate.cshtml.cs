using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Albums
{
    [IgnoreAntiforgeryToken]
    public class RateModel : PageModel
    {
        private readonly VinylShopContext _context;

        public RateModel(VinylShopContext context)
        {
            _context = context;
        }

        public class RateRequest
        {
            public int AlbumId { get; set; }
            public int Rating { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var request = System.Text.Json.JsonSerializer.Deserialize<RateRequest>(body, options);
            if (request == null || request.AlbumId == 0 || request.Rating < 1 || request.Rating > 5)
            {
                return BadRequest();
            }

            var album = await _context.Albums.Include(a => a.Reviews).FirstOrDefaultAsync(a => a.Id == request.AlbumId);
            if (album == null)
            {
                return NotFound();
            }

            var username = User.Identity.IsAuthenticated ? User.Identity.Name ?? "Anonymous" : "Anonymous";

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.AlbumId == request.AlbumId && r.AuthorName == username);

            if (existingReview != null)
            {
                existingReview.Rating = request.Rating;
                existingReview.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                var review = new Review
                {
                    AlbumId = album.Id,
                    AuthorName = username,
                    Rating = request.Rating,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Reviews.Add(review);
            }

            await _context.SaveChangesAsync();

            // Перераховуємо середній рейтинг
            album = await _context.Albums.Include(a => a.Reviews).FirstOrDefaultAsync(a => a.Id == request.AlbumId);

            album.Rating = album.Reviews.Any() ? Math.Round(album.Reviews.Average(r => r.Rating), 1) : 0;
            album.ReviewCount = album.Reviews.Count;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }
    }
}
