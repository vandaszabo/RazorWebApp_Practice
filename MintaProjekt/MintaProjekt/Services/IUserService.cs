using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetUsers();
        Task LogoutUser(string userId);
    }
}
