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
        public List<Album> SimilarAlbums { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int Quantity { get; set; } = 1;

        public string? UserName { get; set; }
        public int? UserRating { get; set; }

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

            Reviews = Album.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            SimilarAlbums = await GetSimilarAlbumsAsync(Album, take: 12);

            if (User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                    {
                        UserName = $"{user.FirstName} {user.LastName}";
                    }
                }

                var username = User.Identity.Name;
                var userReview = Album.Reviews.FirstOrDefault(r => r.AuthorName == username);
                UserRating = userReview?.Rating;
            }

            return Page();
        }

        private async Task<List<Album>> GetSimilarAlbumsAsync(Album current, int take)
        {
            var result = new List<Album>();

            var sameGenre = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Where(a => a.Id != current.Id && a.GenreId == current.GenreId)
                .OrderByDescending(a => a.Rating)
                .Take(take)
                .ToListAsync();
            result.AddRange(sameGenre);

            if (result.Count < take)
            {
                var sameArtist = await _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.Genre)
                    .Where(a => a.Id != current.Id
                        && a.ArtistId == current.ArtistId
                        && !result.Select(r => r.Id).Contains(a.Id))
                    .OrderByDescending(a => a.Rating)
                    .Take(take - result.Count)
                    .ToListAsync();
                result.AddRange(sameArtist);
            }

            if (result.Count < take)
            {
                var topRated = await _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.Genre)
                    .Where(a => a.Id != current.Id && !result.Select(r => r.Id).Contains(a.Id))
                    .OrderByDescending(a => a.Rating)
                    .Take(take - result.Count)
                    .ToListAsync();
                result.AddRange(topRated);
            }

            return result;
        }

        public async Task<IActionResult> OnPostSubmitReviewAsync()
        {
            var albumIdStr = Request.Form["AlbumId"];
            var authorName = Request.Form["AuthorName"];
            var comment = Request.Form["Comment"];

            if (!int.TryParse(albumIdStr, out var albumId))
            {
                return BadRequest();
            }

            var album = await _context.Albums.Include(a => a.Reviews).FirstOrDefaultAsync(a => a.Id == albumId);
            if (album == null)
            {
                return NotFound();
            }

            var review = new Review
            {
                AlbumId = albumId,
                AuthorName = authorName,
                Comment = comment,
                Rating = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            album.Rating = album.Reviews.Any() ? Math.Round(album.Reviews.Average(r => r.Rating), 1) : 0;
            album.ReviewCount = album.Reviews.Count;
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = albumId });
        }
    }
}
