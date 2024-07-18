using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using MintaProjekt.Services;
using MintaProjekt.Models;
using Microsoft.AspNetCore.Mvc;

namespace MintaProjekt.Pages
{
    // Users can access this page without authentication
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public string Username { get; private set; }

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("User");
        }
    }
}
