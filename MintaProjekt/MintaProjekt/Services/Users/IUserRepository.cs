using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services.Users
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetUsers(); // TODO name all method Async for consistency
        Task<IdentityUser?> GetUserById(string userId);
        Task<IEnumerable<string>> GetUserRoles(IdentityUser user);
        Task<bool> RemoveUserRoles(IdentityUser user, IEnumerable<string> existingRoles);
        Task<bool> AddUserRole(IdentityUser user, string newRoleName);
        Task<bool> UpdateUser(IdentityUser user);
        Task<bool> DeleteUser(IdentityUser user);
    }
}
