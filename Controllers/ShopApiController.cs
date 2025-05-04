using Microsoft.AspNetCore.Mvc;
using VinylShop.Data;

namespace VinylShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopApiController : ControllerBase
    {
        private readonly VinylShopContext _context;

        public ShopApiController(VinylShopContext context)
        {
            _context = context;
        }

        [HttpGet("artists")]
        public IActionResult GetArtist()
        {
            var artists = _context.Artists.Select(a => new { a.Id, a.Name }).ToList();
            return Ok(artists);
        }
    }
}
