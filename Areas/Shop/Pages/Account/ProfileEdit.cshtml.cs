using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VinylShop.Data;
using VinylShop.Models;
using VinylShop.Utils;

namespace VinylShop.Areas.Shop.Pages.Account
{
    [Authorize(AuthenticationSchemes = "ShopAuth")]
    public class ProfileEditModel : PageModel
    {
        private readonly VinylShopContext _context;

        public ProfileEditModel(VinylShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string Message { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Введіть ім'я")]
            [StringLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Введіть прізвище")]
            [StringLength(50, ErrorMessage = "Прізвище не може перевищувати 50 символів")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Введіть email")]
            [EmailAddress(ErrorMessage = "Некоректний формат")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Введіть номер телефону")]
            [StringLength(20, ErrorMessage = "Номер телефону не може перевищувати 20 символів")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Введіть пароль")]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Підтвердження пароля")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Паролі не співпадають")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            Input = new InputModel
            {
                FirstName = "",
                LastName = "",
                Email = "",
                PhoneNumber = ""
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;
            user.PhoneNumber = Input.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                user.PasswordHash = PasswordUtils.HashPassword(Input.Password);
            }

            _context.SaveChanges();
            Message = "Ваші дані успішно оновлено!";

            return RedirectToPage("/Account/Profile", new { area = "Shop" });
        }
    }
}
