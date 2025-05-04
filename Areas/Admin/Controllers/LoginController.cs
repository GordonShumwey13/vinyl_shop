using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;

namespace VinylShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly VinylShopContext _context;

        public LoginController(VinylShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            var userAdmin = await _context.UserAdmins
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (userAdmin != null && BCrypt.Net.BCrypt.Verify(password, userAdmin.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userAdmin.Email),
                    new Claim(ClaimTypes.Role, userAdmin.Roles.First().RoleName.ToString())
                };

                var identity = new ClaimsIdentity(claims, "AdminAuth", ClaimTypes.Name, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("AdminAuth", principal);

                return RedirectToAction("Index", "Albums", new { area = "Admin" });
            }

            ModelState.AddModelError("", "Invalid credentials.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            return RedirectToAction("Index", "Albums", new { area = "Admin" });
        }
    }
}
