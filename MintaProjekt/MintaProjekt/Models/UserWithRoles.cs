using Microsoft.AspNetCore.Identity;

namespace MintaProjekt.Models
{
    public record UserWithRoles(IdentityUser User, IEnumerable<string> Roles);

}
