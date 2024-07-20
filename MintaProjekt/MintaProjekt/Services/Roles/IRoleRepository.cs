using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MintaProjekt.Services.Roles
{
    public interface IRoleRepository
    {
        Task<IdentityRole?> FindRoleByIdAsync(string roleId);
        Task<IEnumerable<Claim>> GetClaimsAsync(IdentityRole role);
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> AddClaimAsync(IdentityRole role, Claim claim);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityRole?> FindRoleByNameAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
    }
}
