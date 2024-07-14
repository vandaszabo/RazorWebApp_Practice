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
        
    }
}
