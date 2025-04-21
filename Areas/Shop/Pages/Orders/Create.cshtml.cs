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
    Console.WriteLine("🚀 Start OnPostAsync");

    if (!ModelState.IsValid)
    {
        Console.WriteLine("❌ ModelState is invalid");
        foreach (var entry in ModelState)
        {
            foreach (var error in entry.Value.Errors)
            {
                Console.WriteLine($"❌ ModelState Error: {error.ErrorMessage}");
            }
        }
        return Page();
    }

    if (string.IsNullOrEmpty(CartJson))
    {
        Console.WriteLine("❌ CartJson is empty");
        ModelState.AddModelError("", "Кошик порожній");
        return Page();
    }

    Console.WriteLine("🛒 CartJson received:");
    Console.WriteLine(CartJson);

    List<CartItem> cartItems;
    try
    {
        cartItems = JsonSerializer.Deserialize<List<CartItem>>(CartJson) ?? new();
        Console.WriteLine($"✅ Cart deserialized. Items count: {cartItems.Count}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ JSON десеріалізація не вдалася: " + ex.Message);
        ModelState.AddModelError("", "Невірний формат кошика");
        return Page();
    }

    if (!cartItems.Any())
    {
        Console.WriteLine("❌ Cart is empty after deserialization");
        ModelState.AddModelError("", "Кошик порожній");
        return Page();
    }

    try
    {
        Console.WriteLine($"🔍 Шукаємо покупця з email: {BuyerEmail}");
        var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.Email == BuyerEmail);
        if (buyer == null)
        {
            Console.WriteLine("🆕 Створюємо нового покупця");
            buyer = new Buyer { Email = BuyerEmail };
            _context.Buyers.Add(buyer);
            await _context.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine($"✅ Покупець знайдений, ID = {buyer.Id}");
        }

        var newOrder = new Order
        {
            BuyerId = buyer.Id,
            Phone = Phone,
            OrderDate = DateTime.UtcNow,
            PaymentMethod = PaymentMethod,
            City = City,
            Address = Address,
            Comment = Comment,
            TotalPrice = 0m
        };

        Console.WriteLine("📝 Створюємо замовлення (без позицій)");
        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();
        Console.WriteLine($"✅ Замовлення створено з ID = {newOrder.Id}");

        foreach (var item in cartItems)
        {
            int albumId = item.AlbumId != 0 ? item.AlbumId : item.Id;
            Console.WriteLine($"🔍 Шукаємо альбом з ID = {albumId}");

            var album = await _context.Albums.FindAsync(albumId);
            if (album == null)
            {
                Console.WriteLine($"❌ Альбом ID {albumId} не знайдено");
                ModelState.AddModelError("", $"Товар з ID {albumId} недоступний.");
                return Page();
            }

            if (album.Stock < item.Quantity)
            {
                Console.WriteLine($"❌ Недостатньо в наявності: {album.Title} (залишилось {album.Stock}, потрібно {item.Quantity})");
                ModelState.AddModelError("", $"Товар \"{album.Title}\" — недостатньо в наявності.");
                return Page();
            }

            album.Stock -= item.Quantity;

            var orderItem = new OrderItem
            {
                OrderId = newOrder.Id,
                AlbumId = album.Id,
                Quantity = item.Quantity,
                Price = album.Price * item.Quantity
            };

            _context.OrderItems.Add(orderItem);
            newOrder.TotalPrice += orderItem.Price;

            Console.WriteLine($"✅ Додано позицію: {album.Title} × {item.Quantity} = {orderItem.Price} грн");
        }

        await _context.SaveChangesAsync();
        Console.WriteLine("💾 Усі позиції збережені");

        Response.Cookies.Delete("cart");
        Console.WriteLine("🧼 Кошик очищено з cookies");

        Console.WriteLine("✅ Успішне завершення. Редірект на Success");
        return Redirect("/Shop/Orders/Success");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Помилка при збереженні замовлення:");
        Console.WriteLine(ex.ToString());
        ModelState.AddModelError("", "Виникла помилка при оформленні замовлення. Спробуйте ще раз.");
        return Page();
    }
}

    }
}
