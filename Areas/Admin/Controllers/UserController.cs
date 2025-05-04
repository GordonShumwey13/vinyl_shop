using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VinylShop.Data;
using VinylShop.Enums;
using VinylShop.Models;

namespace VinylShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class UserController : Controller
    {
        private readonly VinylShopContext _context;

        public UserController(VinylShopContext context)
        {
            _context = context;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _context.UserAdmins
                .Include(u => u.Roles)
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/User/Create
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(UserRoleEnum)));
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password, UserRoleEnum role)
        {
            if (_context.UserAdmins.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "Такий користувач вже існує");
                ViewBag.Roles = new SelectList(Enum.GetValues(typeof(UserRoleEnum)));
                return View();
            }

            var user = new UserAdmin
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = new List<UserRole>
                {
                    new UserRole { RoleName = role }
                }
            };

            _context.UserAdmins.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserAdmins
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(UserRoleEnum)), user.Roles.FirstOrDefault()?.RoleName);

            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string email, string password, UserRoleEnum role)
        {
            var user = await _context.UserAdmins
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            user.Email = email;
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            user.Roles.Clear();
            user.Roles.Add(new UserRole { RoleName = role });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.UserAdmins
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.UserAdmins.FindAsync(id);
            if (user != null)
            {
                _context.UserAdmins.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
