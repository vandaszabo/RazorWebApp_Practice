using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MintaProjekt.Services;
using MintaProjekt.Models;
using Microsoft.AspNetCore.Mvc;
using MintaProjekt.Utilities;
using Microsoft.AspNetCore.Identity;

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
    }
}
