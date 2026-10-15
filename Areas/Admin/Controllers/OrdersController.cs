using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Models;
using VinylShop.Models.Enums;

namespace VinylShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class OrdersController : Controller
    {
        private readonly VinylShopContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(VinylShopContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Album)
                        .ThenInclude(a => a.Artist)
                .ToListAsync();

            return View(orders);
        }

        // GET: Admin/Orders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await LoadOrderAsync(id.Value);
            if (order == null) return NotFound();

            return View(BuildEditDto(order));
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var order = await LoadOrderAsync(id);
            if (order == null) return NotFound();

            // Залишок на складі коригуємо лише для активних замовлень:
            // товари відхиленого замовлення вже повернуто на склад
            var affectsStock = order.Status != OrderStatus.Declined;

            foreach (var itemDto in dto.Items)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (item == null) continue;

                if (itemDto.Quantity < 1)
                {
                    ModelState.AddModelError(string.Empty, $"Кількість для \"{item.Album.Title}\" має бути не меншою за 1.");
                }
                else if (affectsStock && itemDto.Quantity - item.Quantity > item.Album.Stock)
                {
                    ModelState.AddModelError(string.Empty, $"Недостатньо \"{item.Album.Title}\" на складі: доступно ще {item.Album.Stock} шт.");
                }
            }

            if (!ModelState.IsValid)
            {
                var invalidDto = BuildEditDto(order);
                invalidDto.Address = dto.Address;
                invalidDto.City = dto.City;
                invalidDto.Phone = dto.Phone;
                foreach (var itemDto in invalidDto.Items)
                {
                    var posted = dto.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (posted != null) itemDto.Quantity = posted.Quantity;
                }
                return View(invalidDto);
            }

            _logger.LogInformation($"Updating order Id={id}");

            order.Address = dto.Address;
            order.City = dto.City;
            order.Phone = dto.Phone;

            foreach (var itemDto in dto.Items)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (item == null || item.Quantity == itemDto.Quantity) continue;

                _logger.LogInformation($"Updating quantity for item Id={itemDto.Id} to {itemDto.Quantity}");

                if (affectsStock)
                {
                    item.Album.Stock -= itemDto.Quantity - item.Quantity;
                }

                item.Price = UnitPrice(item) * itemDto.Quantity;
                item.Quantity = itemDto.Quantity;
            }

            order.TotalPrice = order.Items.Sum(i => i.Price);
            _logger.LogInformation($"Updated total price: {order.TotalPrice}");

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Order Id={id} saved");

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Orders/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.OrderItems.FindAsync(itemId);
            if (item == null) return NotFound();

            return await RemoveItemFromOrder(item.OrderId, itemId);
        }

        // POST: Admin/Orders/DeleteItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int orderId, int itemId)
        {
            return await RemoveItemFromOrder(orderId, itemId);
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            if (!Enum.IsDefined(status)) return BadRequest();

            var order = await LoadOrderAsync(id);
            if (order == null) return NotFound();

            if (order.Status != OrderStatus.Declined && status == OrderStatus.Declined)
            {
                // Відхилене замовлення повертає товари на склад
                foreach (var item in order.Items)
                {
                    item.Album.Stock += item.Quantity;
                }
            }
            else if (order.Status == OrderStatus.Declined && status != OrderStatus.Declined)
            {
                // Відновлене замовлення знову забирає товари зі складу
                var missing = order.Items.FirstOrDefault(i => i.Album.Stock < i.Quantity);
                if (missing != null)
                {
                    TempData["ErrorMessage"] = $"Замовлення #{order.Id} не можна відновити: недостатньо \"{missing.Album.Title}\" на складі.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var item in order.Items)
                {
                    item.Album.Stock -= item.Quantity;
                }
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> RemoveItemFromOrder(int orderId, int itemId)
        {
            _logger.LogInformation($"Deleting item Id={itemId} from order Id={orderId}");

            var order = await LoadOrderAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning($"Order Id={orderId} not found");
                return NotFound();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                _logger.LogWarning($"Item Id={itemId} not found in order Id={orderId}");
                return RedirectToAction(nameof(Edit), new { id = orderId });
            }

            if (order.Status != OrderStatus.Declined)
            {
                item.Album.Stock += item.Quantity;
            }

            order.Items.Remove(item);
            _context.OrderItems.Remove(item);
            order.TotalPrice = order.Items.Sum(i => i.Price);

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Item removed, updated total price: {order.TotalPrice}");

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }

        private Task<Order?> LoadOrderAsync(int id)
        {
            return _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Album)
                        .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        private static OrderEditDto BuildEditDto(Order order)
        {
            return new OrderEditDto
            {
                Id = order.Id,
                Address = order.Address,
                City = order.City,
                Phone = order.Phone,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    AlbumId = item.AlbumId,
                    AlbumTitle = item.Album.Title,
                    ArtistName = item.Album.Artist.Name,
                    Quantity = item.Quantity
                }).ToList()
            };
        }

        // Price позиції зберігає суму за всю кількість на момент замовлення
        private static decimal UnitPrice(OrderItem item)
        {
            return item.Quantity > 0 ? item.Price / item.Quantity : item.Album.Price;
        }
    }
}
