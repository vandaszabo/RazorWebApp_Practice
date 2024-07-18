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
        private readonly ILogger<IndexModel> _logger;
        public IdentityUser? CurrentUser { get; private set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {
            CurrentUser = HttpContext.Session.GetObjectFromJson<IdentityUser>("User");
            if (CurrentUser != null)
            {
                _logger.LogInformation("Logged-In User: {CurrentUser} ", CurrentUser.ToString());
            }
            else
            {
                _logger.LogInformation("No User Signed In.");
            }
        }

        public IActionResult OnPostSetLanguage(string culture)
        {
            _logger.LogInformation("Selected language: {culture} ", culture);
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToPage(); // Redirect to refresh the page with the new culture
        }
    }
}
