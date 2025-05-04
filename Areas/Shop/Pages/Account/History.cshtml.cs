using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Account
{
    [Authorize(AuthenticationSchemes = "ShopAuth")]
    public class HistoryModel : PageModel
    {
        private readonly VinylShopContext _context;

        public HistoryModel(VinylShopContext context)
        {
            _context = context;
        }

        public List<Order> Orders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out var userId))
            {
                return RedirectToPage("/Account/Login");
            }

            Orders = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Album)
                        .ThenInclude(a => a.Artist)
                .Where(o => o.Buyer.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Page();
        }
    }
}
