using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MintaProjekt.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;


        public UserRepository(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Retrieve all users
        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        // Get user by Id
        public async Task<IdentityUser?> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        // Get user roles
        public async Task<IEnumerable<string>> GetUserRoles(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        // Remove existing roles
        public async Task<bool> RemoveUserRoles(IdentityUser user, IEnumerable<string> existingRoles)
        {
            var result = await _userManager.RemoveFromRolesAsync(user, existingRoles);
            return result.Succeeded;
        }

        // Add new role
        public async Task<bool> AddUserRole(IdentityUser user, string newRoleName)
        {
            var result = await _userManager.AddToRoleAsync(user, newRoleName);
            return result.Succeeded;
        }

        // Update user
        public async Task<bool> UpdateUser(IdentityUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // Delete a user
        public async Task<bool> DeleteUser(IdentityUser user)
        {
           var result = await _userManager.DeleteAsync(user);
           return result.Succeeded;
        }

    }
}
