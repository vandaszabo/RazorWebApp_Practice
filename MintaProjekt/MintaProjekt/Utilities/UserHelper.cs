using System.Security.Claims;

namespace MintaProjekt.Utilities
{
    public static class UserHelper
    {
        // Get Current User's ID
        public static string GetCurrentUserID(ClaimsPrincipal user)
        {
            // Find all User Claims with type: ClaimTypes.NameIdentifier
            var userClaims = user.FindAll(ClaimTypes.NameIdentifier).ToList();

            // Check if no claims found 
            if (userClaims.Count == 0)
            {
                throw new InvalidOperationException("User ID claim is not found.");
            }

            // Check if more than one found
            if (userClaims.Count > 1)
            {
                throw new InvalidOperationException("Multiple User ID claims found.");
            }

            // Retrieve UserID from Claim
            var currentUserIDClaim = userClaims.Single();
            var currentUserID = currentUserIDClaim.Value;

            return currentUserID;
        }
    }
}
