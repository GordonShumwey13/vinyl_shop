using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly VinylShopContext _context;

        public ProfileModel(VinylShopContext context)
        {
            _context = context;
        }

        public string UserEmail { get; set; }
        public string FirstName { get; set; }
        public List<Order> Orders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Shop" });
            }

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Shop" });
            }

            UserEmail = user.Email;
            FirstName = user.FirstName;

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (buyer != null)
            {
                Orders = await _context.Orders
                    .Where(o => o.BuyerId == buyer.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Home/Index", new { area = "Shop" });
        }
    }
}
