using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MintaProjekt.Pages.Base
{
    public class BasePageModel : PageModel
    {
        protected readonly UserManager<IdentityUser> _userManager;
        public string? UserId { get; private set; }

        public BasePageModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task GetUserIdAsync()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    UserId = user.Id;
                }
            }
        }

    }
}
