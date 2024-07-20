using Microsoft.AspNetCore.Identity;
using MintaProjekt.Data;
using MintaProjekt.DbContext;

namespace MintaProjekt.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task LogoutUserAsync(string userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user != null)
            {
                user.SecurityStamp = Guid.NewGuid().ToString(); // Update User's Security Stamp in AspNetUsers table
                await _userRepository.UpdateUser(user);
            }
        }
    }

}
