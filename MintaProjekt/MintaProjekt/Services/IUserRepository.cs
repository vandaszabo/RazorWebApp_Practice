using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Data
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task<IdentityUser?> GetUserById(string userId);
        Task<bool> UpdateUser(IdentityUser user);
        Task<bool> DeleteUser(string userId);
    }
}
