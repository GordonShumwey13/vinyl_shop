using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using VinylShop.Data;
using VinylShop.Utils;

namespace VinylShop.Areas.Shop.Pages.Account
{
    [Authorize]
    public class ProfileEditModel : PageModel
    {
        private readonly VinylShopContext _context;

        public ProfileEditModel(VinylShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Message { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле Email обов'язкове")]
            [EmailAddress(ErrorMessage = "Некоректний Email")]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Паролі не співпадають.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            Input = new InputModel
            {
                Email = user.Email
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.Find(userId);

            user.Email = Input.Email;

            if (!string.IsNullOrWhiteSpace(Input.NewPassword))
            {
                user.PasswordHash = PasswordUtils.HashPassword(Input.NewPassword);
            }

            _context.SaveChanges();
            Message = "Ваші дані успішно оновлено!";

            return Page();
        }
    }
}
