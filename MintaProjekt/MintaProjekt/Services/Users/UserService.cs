using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MintaProjekt.Services.Roles;

namespace MintaProjekt.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<IUserService> _logger;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ILogger<IUserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }


        // Retrieve all users
        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            _logger.LogInformation("Start GetUsers method.");
            try
            {
                var users = await _userRepository.GetUsers();
                return users;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService GetUsers method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }

        // Get user by id
        public async Task<IdentityUser> GetUserByID(string userId)
        {
            _logger.LogInformation("Start GetUserByID method.");
            try
            {
                IdentityUser? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found in GetUserByID method.", userId);
                    throw new InvalidOperationException($"User with ID {userId} not found.");
                }
                return user;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService GetUserByID method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }

        // Get user roles
        public async Task<IEnumerable<string>> GetUserRoles(IdentityUser user)
        {
            _logger.LogInformation("Start GetUserRoles method.");
            try
            {
                var roleNames = await _userRepository.GetUserRoles(user);
                return roleNames;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService GetUserRoles method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }

        // Update user
        public async Task UpdateUserProperties(IdentityUser user)
        {
            _logger.LogInformation("Start UpdateUserProperties method.");
            var existingUser = await GetUserByID(user.Id);

            // Check concurrent editing
            if (existingUser.ConcurrencyStamp != user.ConcurrencyStamp)
            {
                throw new DbUpdateConcurrencyException("The user was modified by another process.");
            }

            // Update properties
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            // Save changes
            await _userRepository.UpdateUser(existingUser);
        }


        // Logout specific user
        public async Task LogoutUser(string userID)
        {
            _logger.LogInformation("Start LogoutUser method.");
            try
            {
                var user = await GetUserByID(userID);
                user.SecurityStamp = Guid.NewGuid().ToString(); // Update User's Security Stamp in AspNetUsers table
                await _userRepository.UpdateUser(user);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService LogoutUser method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }

        // Change User's Role
        public async Task ChangeUserRole(string userId, string newRole)
        {
            _logger.LogInformation("Start ChangeUserRole method.");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newRole))
            {
                _logger.LogWarning("UserId or newRole parameter is null or empty in ChangeUserRole method.");
                throw new ArgumentException("Received parameters cannot be null or empty in ChangeUserRole method.");
            }

            try
            {
                // Get the user
                var user = await GetUserByID(userId);

                // Check if the new role exists
                var roleExists = await _roleRepository.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    _logger.LogWarning("Role {NewRole} does not exist in the database.", newRole);
                    throw new InvalidOperationException($"Role {newRole} does not exist.");
                }

                // Get the existing roles
                var existingRoles = await _userRepository.GetUserRoles(user);

                // Check if the role is already assigned
                if (existingRoles.Contains(newRole))
                {
                    _logger.LogInformation("User with ID {UserId} already has the role {NewRole}. No change needed.", userId, newRole);
                    return;
                }

                // Remove all existing roles if any
                if (existingRoles.Any())
                {
                    await RemoveExistingRoles(user, existingRoles);
                }

                // Add the new role
                await AddNewRole(user, newRole);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in ChangeUserRoleAsync method.");
                throw new InvalidOperationException("An error occurred while changing the user role.", ex);
            }
        }

        // Remove User Roles
        private async Task RemoveExistingRoles(IdentityUser user, IEnumerable<string> existingRoles)
        {
            _logger.LogInformation("Start RemoveExistingRoles method.");
            var success = await _userRepository.RemoveUserRoles(user, existingRoles);
            if (!success)
            {
                _logger.LogWarning("Failed to remove existing roles for user with ID {UserId}.", user.Id);
                throw new InvalidOperationException("Failed to remove existing roles.");
            }
        }

        // Add User Role
        private async Task AddNewRole(IdentityUser user, string newRole)
        {
            _logger.LogInformation("Start AddNewRole method.");
            var success = await _userRepository.AddUserRole(user, newRole);
            if (!success)
            {
                _logger.LogWarning("Failed to add new role {NewRole} for user with ID {UserId}.", newRole, user.Id);
                throw new InvalidOperationException("Failed to add new role.");
            }
            _logger.LogInformation("New role {NewRole} added successfully for user with ID {UserId}.", newRole, user.Id);
        }
    }

}
