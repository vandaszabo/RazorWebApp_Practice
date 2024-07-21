using Microsoft.AspNetCore.Authorization;
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
        private readonly IUserTransactions _userTransactions;
        public IEnumerable<RoleWithClaims> RolesWithClaims { get; private set; }
        public IEnumerable<UserWithRoles> UsersWithRoles { get; private set; }

        [BindProperty]
        public string? SelectedUserID { get; set; }
        [BindProperty]
        public string? SelectedRole { get; set; }

        public AdminModel(IUserService userService, IRoleService roleService, IUserTransactions userTransactions)
        {
            _userService = userService;
            _roleService = roleService;
            RolesWithClaims = new List<RoleWithClaims>();
            UsersWithRoles = new List<UserWithRoles>();
            _userTransactions = userTransactions;
        }

        // Get all Users and Roles
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Users with their assigned roles
                UsersWithRoles = await _userService.GetUsersWithRoles();

                // Roles with claims
                RolesWithClaims = await _roleService.GetRolesWithClaims();
                return Page();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving users and roles.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateAsync()
        {
            if (SelectedUserID == null || SelectedRole == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid selection.");
                return await OnGetAsync();
            }
            try
            {
                // Update user role
                await _userTransactions.ExecuteUserRoleChangeAndLogoutAsync(SelectedUserID, SelectedRole);
                return await OnGetAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating user role.");
                return Page();
            }
        }


    }
}
