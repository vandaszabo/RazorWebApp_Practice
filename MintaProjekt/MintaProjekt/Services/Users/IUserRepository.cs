using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services.Users
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IdentityUser?> GetUserById(string userId);
        Task<IEnumerable<string>> GetUserRoles(IdentityUser user);
        Task<bool> UpdateUser(IdentityUser user);
        Task<bool> DeleteUser(string userId);
    }
}
