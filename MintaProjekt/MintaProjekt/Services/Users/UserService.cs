using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IUserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<IUserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }


        // Retrieve all users
        public async Task<IEnumerable<IdentityUser>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsers();
                _logger.LogInformation("Number of users retrieved: {users.Count}", users.Count());
                return users;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService GetUsers method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }

        // Get user roles
        public async Task<IEnumerable<string>> GetUserRoles(IdentityUser user)
        {
            _logger.LogDebug("Try to get role names for user {userID} ", user.Id);
            try
            {
                var roleNames = await _userRepository.GetUserRoles(user);
                _logger.LogInformation("Number of roles found for user: {roles.Count}", roleNames.Count());
                return roleNames;

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Exeption occured in UserService GetUserRoles method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }


        // Logout specific user
        public async Task LogoutUser(string userID)
        {
            try
            {
                var user = await _userRepository.GetUserById(userID);
                if (user == null)
                {
                    _logger.LogWarning("Cannot logout user! Retrieved user is null. {userID} ", userID);
                }
                else
                {
                    user.SecurityStamp = Guid.NewGuid().ToString(); // Update User's Security Stamp in AspNetUsers table
                    await _userRepository.UpdateUser(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in UserService LogoutUser method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }
    }

}
