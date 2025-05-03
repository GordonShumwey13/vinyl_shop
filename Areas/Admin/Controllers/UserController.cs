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
    [Authorize(Roles = nameof(UserRoleEnum.Admin))]
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
            var users = await _context.Users
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
        public async Task<IActionResult> Create(string email, string password, string firstName, string lastName, UserRoleEnum role)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "Такий користувач вже існує");
                ViewBag.Roles = new SelectList(Enum.GetValues(typeof(UserRoleEnum)));
                return View();
            }

            var user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = "",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Roles = new List<UserRole>
                {
                    new UserRole { RoleName = role }
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(Enum.GetValues(typeof(UserRoleEnum)), user.Roles.FirstOrDefault()?.RoleName);

            return View(user);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string email, string firstName, string lastName, string password, UserRoleEnum role)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;
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

            var user = await _context.Users
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
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
