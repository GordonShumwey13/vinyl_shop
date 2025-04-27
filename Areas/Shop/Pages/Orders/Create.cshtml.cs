using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VinylShop.Data;
using VinylShop.Models;

namespace VinylShop.Areas.Shop.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly VinylShopContext _context;

        public CreateModel(VinylShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string BuyerEmail { get; set; } = string.Empty;

        [BindProperty]
        public string PaymentMethod { get; set; } = string.Empty;

        [BindProperty]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        public string Comment { get; set; } = string.Empty;

        [BindProperty]
        public string CartJson { get; set; } = string.Empty;

        public Album? Album { get; set; }
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync(int? albumId, int quantity = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;
                if (userId.HasValue)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                    if (user != null)
                    {
                        Phone = user.PhoneNumber ?? string.Empty;
                    }
                }
            }

            if (albumId.HasValue)
            {
                Album = await _context.Albums
                    .Include(a => a.Artist)
                    .Include(a => a.Genre)
                    .FirstOrDefaultAsync(a => a.Id == albumId.Value);

                if (Album == null)
                {
                    return NotFound();
                }

                Quantity = quantity;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(CartJson))
            {
                ModelState.AddModelError(string.Empty, "Кошик порожній.");
                return Page();
            }

            List<CartItem> cartItems;
            try
            {
                cartItems = JsonSerializer.Deserialize<List<CartItem>>(CartJson) ?? new();
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Невірний формат кошика.");
                return Page();
            }

            if (!cartItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Кошик порожній.");
                return Page();
            }

            var (email, userId) = GetUserInfo();
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError(string.Empty, "Не вказаний email.");
                return Page();
            }

            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == email);
            if (buyer == null)
            {
                buyer = new Buyer { Email = email, UserId = userId };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }
            else if (userId.HasValue && buyer.UserId == null)
            {
                buyer.UserId = userId;
                _context.Buyers.Update(buyer);
                await _context.SaveChangesAsync();
            }

            var resolvedPhone = Phone;
            if (User.Identity.IsAuthenticated && userId.HasValue)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
                if (user != null)
                {
                    resolvedPhone = user.PhoneNumber;
                }
            }

            var newOrder = new Order
            {
                BuyerId = buyer.Id,
                Phone = resolvedPhone,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = PaymentMethod,
                City = City,
                Address = Address,
                Comment = Comment,
                TotalPrice = 0m,
                Items = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                int albumId = item.AlbumId != 0 ? item.AlbumId : item.Id;
                var album = await _context.Albums.FindAsync(albumId);

                if (album == null || album.Stock < item.Quantity)
                {
                    ModelState.AddModelError(string.Empty, $"Товар \"{album?.Title ?? "невідомий"}\" недоступний або немає в наявності.");
                    return Page();
                }

                album.Stock -= item.Quantity;

                newOrder.Items.Add(new OrderItem
                {
                    AlbumId = album.Id,
                    Quantity = item.Quantity,
                    Price = album.Price * item.Quantity
                });

                newOrder.TotalPrice += album.Price * item.Quantity;
            }

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            Response.Cookies.Delete("cart");

            TempData["SuccessMessage"] = "Замовлення успішно оформлено!";
            return Redirect("/Shop/Account/Profile");
        }

        private (string? Email, int? UserId) GetUserInfo()
        {
            string? email = null;
            int? userId = null;

            if (User.Identity.IsAuthenticated)
            {
                email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                userId = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var uid) ? uid : (int?)null;
            }

            if (string.IsNullOrEmpty(email))
            {
                email = BuyerEmail;
            }

            return (email, userId);
        }
    }
}
