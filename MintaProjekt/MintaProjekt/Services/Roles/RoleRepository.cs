using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace MintaProjekt.Services.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // Get claims for role
        public async Task<IEnumerable<Claim>> GetClaimsAsync(IdentityRole role)
        {
           return await _roleManager.GetClaimsAsync(role);
        }

        // Add claim for role
        public async Task<IdentityResult> AddClaimAsync(IdentityRole role, Claim claim)
        {
            return await _roleManager.AddClaimAsync(role, claim);
        }

        // Check role if exists
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        // Create new role
        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            return await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Get all roles
        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        // Find role by ID
        public async Task<IdentityRole?> FindRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        // Find role by name
        public async Task<IdentityRole?> FindRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        // Delete a role
        public async Task<IdentityResult> DeleteRoleAsync(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }

        // Update a role
        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }
    }
}
