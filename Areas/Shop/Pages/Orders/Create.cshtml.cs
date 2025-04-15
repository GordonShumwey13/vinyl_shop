using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;
using System.Text.Json;

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

        public Album? Album { get; set; }
        public int Quantity { get; set; } = 1;

        // Create Order - GET
        public async Task<IActionResult> OnGetAsync(int? albumId, int quantity = 1)
        {
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

        // Create Order - POST
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cartJson = Request.Cookies["cart"];
            if (string.IsNullOrEmpty(cartJson))
            {
                ModelState.AddModelError("", "Кошик порожній");
                return Page();
            }

            List<CartItem> cartItems;
            try
            {
                cartItems = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new();
            }
            catch
            {
                ModelState.AddModelError("", "Невірний формат кошика");
                return Page();
            }

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Кошик порожній");
                return Page();
            }

            // Пошук або створення покупця
            var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == BuyerEmail);
            if (buyer == null)
            {
                buyer = new Buyer { Email = BuyerEmail };
                _context.Buyers.Add(buyer);
                await _context.SaveChangesAsync();
            }

            // Створення нового замовлення
            var newOrder = new Order
            {
                BuyerId = buyer.Id,
                OrderDate = DateTime.UtcNow,
                TotalPrice = 0m
            };

            // Додаємо товарні записи до замовлення
            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (var item in cartItems)
            {
                var album = await _context.Albums.FindAsync(item.AlbumId);
                if (album == null || album.Stock < item.Quantity)
                {
                    ModelState.AddModelError("", $"Товар з ID {item.AlbumId} недоступний або недостатньо в наявності.");
                    return Page();
                }

                album.Stock -= item.Quantity;

                // Додаємо товарний запис для кожного товару в кошику
                var orderItem = new OrderItem
                {
                    AlbumId = album.Id,
                    Quantity = item.Quantity,
                    Price = album.Price * item.Quantity // Загальна ціна за цей товар
                };

                orderItems.Add(orderItem);
                newOrder.TotalPrice += orderItem.Price; // Додаємо в загальну вартість замовлення
            }

            // Додаємо замовлення в базу
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Додаємо товарні записи в базу
            foreach (var orderItem in orderItems)
            {
                orderItem.OrderId = newOrder.Id; // Прив'язуємо до конкретного замовлення
                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();

            // Очищаємо кошик після оформлення замовлення
            Response.Cookies.Delete("cart");

            // Переадресація на сторінку успішного оформлення
            return RedirectToPage("Success");
        }
    }
}