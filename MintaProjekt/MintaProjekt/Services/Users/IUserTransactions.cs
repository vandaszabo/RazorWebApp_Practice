namespace MintaProjekt.Services.Users
{
    public interface IUserTransactions
    {
        Task ExecuteUserRoleChangeAndLogoutAsync(string userId, string role);
    }
}
