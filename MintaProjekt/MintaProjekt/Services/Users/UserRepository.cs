using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MintaProjekt.Services.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserRepository> _logger;


        public UserRepository(UserManager<IdentityUser> userManager, ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // Retrieve all users
        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        // Retrieve a specific user by Id
        public async Task<IdentityUser?> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        // Get user roles
        public async Task<IEnumerable<string>> GetUserRoles(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        // Update user properties
        public async Task<bool> UpdateUser(IdentityUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        // Delete a user by Id
        public async Task<bool> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return false;
        }

        // Change user's password
        //public async Task ChangeUserPassword(IdentityUser user, string newPassword)
        //{
        //    try
        //    {
        //        var newPasswordHash = newPassword != null ? newPassword.GetHashCode() : 0;
        //        user.PasswordHash = newPasswordHash;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exeption occured in UserService - ChangeUserPassword method.");
        //        throw;
        //    }

        //}

        // Change user's role



    }
}
