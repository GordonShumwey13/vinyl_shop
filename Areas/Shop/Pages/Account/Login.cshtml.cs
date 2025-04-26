#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using VinylShop.Data;
using VinylShop.Utils;

namespace VinylShop.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly VinylShopContext _context;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(VinylShopContext context, ILogger<LoginModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Введіть email")]
            [EmailAddress(ErrorMessage = "Некоректний формат")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Введіть пароль")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            var user = _context.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (user == null || !PasswordUtils.Verify(Input.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Невірна електронна пошта або пароль.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("Користувач успішно увійшов.");

            ViewData["LoginSuccess"] = true;
            return Page();
        }
    }
}
