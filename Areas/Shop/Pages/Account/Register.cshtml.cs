#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VinylShop.Data;
using VinylShop.Models;
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

            [Required(ErrorMessage = "Підтвердження пароля")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Паролі не співпадають")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Введіть ім'я")]
            [StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Введіть прізвище")]
            [StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Введіть номер телефону")]
            [StringLength(20, ErrorMessage = "Номер телефону не може перевищувати 20 символів")]
            public string PhoneNumber { get; set; }
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
                PasswordHash = PasswordUtils.HashPassword(Input.Password),
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                PhoneNumber = Input.PhoneNumber,
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
            return RedirectToPage("/Albums/Albums");
        }
    }
}
