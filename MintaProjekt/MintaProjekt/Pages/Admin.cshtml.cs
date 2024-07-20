using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MintaProjekt.Models;
using MintaProjekt.Services.Roles;
using MintaProjekt.Services.Users;

namespace MintaProjekt.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public IEnumerable<RoleWithClaims> RolesWithClaims { get; private set; }
        public IEnumerable<UserWithRoles> UsersWithRoles { get; private set; }

        public AdminModel(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
            RolesWithClaims = new List<RoleWithClaims>();
            UsersWithRoles = new List<UserWithRoles>();
        }

        // Get all Users and Roles
        public async Task<IActionResult> OnGet()
        {
            try
            {
                // Users with their assigned roles
                var users = await _userService.GetUsers();
                var usersWithRoles = new List<UserWithRoles>();

                foreach (var user in users)
                {
                    var userRoles = await _userService.GetUserRoles(user);
                    usersWithRoles.Add(new UserWithRoles(user, userRoles));
                }

                UsersWithRoles = usersWithRoles;

                // Roles with claims
                var roles = await _roleService.GetAllRoles();

                var rolesWithClaims = new List<RoleWithClaims>();
                foreach (var role in roles)
                {
                    var claims = await _roleService.GetClaimsForRoleAsync(role.Id);
                    rolesWithClaims.Add(new RoleWithClaims(role, claims));
                }

                RolesWithClaims = rolesWithClaims;
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving users.");
                return Page();
            }
        }


    }
}
