using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MintaProjekt.Services.Roles
{
    public interface IRoleService
    {
        Task<IdentityRole> CreateRole(string roleName);
        Task AddClaimIfNotExists(IdentityRole role, Claim claim);
        Task AddClaimsToRole(IdentityRole role, List<Claim> claims);
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        Task<IEnumerable<Claim>> GetClaimsForRoleAsync(string roleID);
    }
}
