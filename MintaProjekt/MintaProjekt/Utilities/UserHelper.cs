using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MintaProjekt.Utilities
{
    public class UserHelper
    {
        // TODO Change code for retrive userID from session

        private readonly UserManager<IdentityUser> _userManager;
        public UserHelper(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Get Current User's ID
        public async Task<string> GetCurrentUserIDAsync(ClaimsPrincipal user)
        {
            // Get the userName
            var userIdentity = user.Identity ?? throw new InvalidOperationException("User Identity not found.");
            var userName = userIdentity.Name ?? throw new InvalidOperationException("User's Name not found.");

            // Use UserManager to find the user by name
            var identityUser = await _userManager.FindByNameAsync(userName) ?? throw new InvalidOperationException("User not found.");

            // Return the user's ID
            return identityUser.Id;
        }
    }
}
