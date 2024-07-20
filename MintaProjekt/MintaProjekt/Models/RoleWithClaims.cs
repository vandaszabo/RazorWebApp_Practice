using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MintaProjekt.Models
{
    public record RoleWithClaims(IdentityRole Role, IEnumerable<Claim> Claims);
}
