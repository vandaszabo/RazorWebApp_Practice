using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IEnumerable<string>> GetUserRoles(IdentityUser user);
        Task LogoutUser(string userId);
    }
}
