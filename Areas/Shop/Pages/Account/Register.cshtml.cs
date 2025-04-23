#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VinylShop.Data;
using VinylShop.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using VinylShop.Utils;

namespace VinylShop.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly VinylShopContext _context;

        public RegisterModel(VinylShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Введіть email")]
            [EmailAddress(ErrorMessage = "Некоректний формат")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Введіть пароль")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Паролі не співпадають")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (_context.Users.Any(u => u.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "Користувач з таким email вже існує.");
                return Page();
            }

            var user = new User
            {   
                Email = Input.Email,
                PasswordHash = PasswordUtils.HashPassword(Input.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            ViewData["RegistrationSuccess"] = true;
            return Page();
        }
    }
}
