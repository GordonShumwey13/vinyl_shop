using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace VinylShop.Areas.Shop.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public string UserEmail { get; set; }

        public void OnGet()
        {
            UserEmail = User.FindFirstValue(ClaimTypes.Email);
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Shop/Home");
        }
    }
}
