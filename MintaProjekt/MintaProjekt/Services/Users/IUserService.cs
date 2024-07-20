using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IEnumerable<string>> GetUserRoles(IdentityUser user);
        Task UpdateUserProperties(IdentityUser user);
        Task LogoutUser(string userId);
        Task ChangeUserRole(string userId, string newRole);
    }
}
