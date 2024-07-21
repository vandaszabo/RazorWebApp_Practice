using Microsoft.AspNetCore.Identity;
using MintaProjekt.Models;

namespace MintaProjekt.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IEnumerable<UserWithRoles>> GetUsersWithRoles();
        Task<IdentityUser> GetUserByID(string userId);
        Task<IEnumerable<string>> GetUserRoles(IdentityUser user);
        Task UpdateUserProperties(IdentityUser user);
        Task LogoutUser(string userId);
        Task ChangeUserRole(string userId, string newRole);
        Task AddNewRole(IdentityUser user, string newRole);
    }
}
