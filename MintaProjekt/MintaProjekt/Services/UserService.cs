using Microsoft.AspNetCore.Identity;
using MintaProjekt.Data;
using MintaProjekt.DbContext;

namespace MintaProjekt.Services
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
