namespace MintaProjekt.Services
{
    public interface IUserService
    {
        Task LogoutUserAsync(string userId);
    }
}
