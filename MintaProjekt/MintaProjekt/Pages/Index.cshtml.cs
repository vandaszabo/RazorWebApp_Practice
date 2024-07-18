using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MintaProjekt.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;

namespace MintaProjekt.Pages
{
    // Users can access this page without authentication
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public IdentityUser? CurrentUser { get; private set; }

        public void OnGet()
        {
            CurrentUser = HttpContext.Session.GetObjectFromJson<IdentityUser>("User");
        }

        public IActionResult OnPostSetLanguage(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Page();
        }
    }
}
