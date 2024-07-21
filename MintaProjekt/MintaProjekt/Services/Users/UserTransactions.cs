using System.Transactions;

namespace MintaProjekt.Services.Users
{
    public class UserTransactions : IUserTransactions
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserTransactions> _logger;

        public UserTransactions(IUserService userService, ILogger<UserTransactions> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // Change role and Logout user
        public async Task ExecuteUserRoleChangeAndLogoutAsync(string userId, string role)
        {
            // Create transaction Scope
                // TransactionScopeAsyncFlowOption.Enabled <= allow asynchronous operations within the transaction.
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // Update role
                await _userService.ChangeUserRole(userId, role);

                // Logout user
                await _userService.LogoutUser(userId);

                // Complete transaction
                    // This call informs the transaction manager that it can commit the transaction.
                    // If an exception occurs or scope.Complete() is not called, the transaction manager rolls back the transaction,
                    // discarding all changes made within the scope.
                scope.Complete();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing user role and logging out.");
                // Transaction is automatically rolled back if not completed
                throw;
            }
        }
    }
}
