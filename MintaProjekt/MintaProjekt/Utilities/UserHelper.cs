using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Utilities
{
    public class UserHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserHelper> _logger;

        public UserHelper(IHttpContextAccessor httpContextAccessor, ILogger<UserHelper> helper)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = helper;
        }

        // Get Current User
        public IdentityUser? GetCurrentUser()
        {
            if(_httpContextAccessor.HttpContext == null)
            {
                _logger.LogWarning("Cannot access HttpContext.");
                return null;
            }
            try
            {
                var user = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<IdentityUser>("User");

                if(user != null)
                {
                    _logger.LogInformation("Current User's Name: {UserName}.", user.UserName);
                    return user;
                }

                _logger.LogWarning("Current User not found.");
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}
