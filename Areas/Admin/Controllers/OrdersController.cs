using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VinylShop.Data;
using VinylShop.Models;
using VinylShop.Models.Enums;

namespace VinylShop.Areas.Admin.Controllers
{
    [Area("Admin")]
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

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Album)
                        .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(OrderStatus)), order.Status);

            var orderDto = new OrderEditDto
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

            return View(orderDto);
        }

        // POST: Admin/Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Album)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            _logger.LogInformation($"Updating order Id={id}");

            order.Address = dto.Address;
            order.City = dto.City;
            order.Phone = dto.Phone;

            foreach (var itemDto in dto.Items)
            {
                var item = order.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                if (item != null)
                {
                    _logger.LogInformation($"Updating quantity for item Id={itemDto.Id} to {itemDto.Quantity}");
                    item.Quantity = itemDto.Quantity;
                }
            }

            order.TotalPrice = order.Items.Sum(i => i.Quantity * i.Album.Price);
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
            if (item != null)
            {
                var orderId = item.OrderId;
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = orderId });
            }
            return NotFound();
        }

        // POST: Admin/Orders/DeleteItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int orderId, int itemId)
        {
            _logger.LogInformation($"Deleting item Id={itemId} from order Id={orderId}");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogWarning($"Order Id={orderId} not found");
                return NotFound();
            }

            var item = order.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _context.OrderItems.Remove(item);
                order.TotalPrice = order.Items.Where(i => i.Id != itemId).Sum(i => i.Quantity * i.Album.Price);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Item removed, updated total price: {order.TotalPrice}");
            }
            else
            {
                _logger.LogWarning($"Item Id={itemId} not found in order Id={orderId}");
            }

            return RedirectToAction(nameof(Edit), new { id = orderId });
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = (OrderStatus)status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
