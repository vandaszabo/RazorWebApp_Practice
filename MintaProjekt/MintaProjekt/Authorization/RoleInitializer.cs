using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Security.Claims;

namespace MintaProjekt.Authorization
{
    public static class RoleInitializer
    {
        // Add Roles
        public static void AddRoles(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                var adminRole = CreateRole(roleManager, "Admin").Result;
                AddClaimsToAdminRole(roleManager, adminRole).Wait();

                var userRole = CreateRole(roleManager, "User").Result;
                AddClaimsToUserRole(roleManager, userRole).Wait();

                var managerRole = CreateRole(roleManager, "Manager").Result;
                AddClaimsToManagerRole(roleManager, managerRole).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or setting up roles.");
            }
        }

        // Create Roles
        private static async Task<IdentityRole> CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            try
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                // create role if not exist
                if (!roleExist)
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        Log.Logger.Information("Role {RoleName} created successfully", roleName);
                    }
                    else
                    {
                        var errorMessage = $"Error occurred while creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                        Log.Logger.Error(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                }
                // Find the previously created role, return it
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    var errorMessage = $"Role {roleName} was not found after creation.";
                    Log.Logger.Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                return role;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or retrieving the role {RoleName}", roleName);
                throw;
            }
        }

        // Add claim to role if it does not exist
        private static async Task AddClaimIfNotExists(RoleManager<IdentityRole> roleManager, IdentityRole role, Claim claim)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            if (!claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                var result = await roleManager.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    var errorMessage = $"Error occurred while adding claim {claim.Type} with value {claim.Value} to role {role.Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                    Log.Logger.Error(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        // Create Claims for Admin Role
        private static async Task AddClaimsToAdminRole(RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            try
            {
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Select"));
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Add"));
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Update"));
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Delete"));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while adding claims to the Admin role.");
                throw;
            }
        }

        // Create Claims for User Role
        private static async Task AddClaimsToUserRole(RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            try
            {
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Select"));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while adding claims to the User role.");
                throw;
            }
        }

        // Create Claims for Manager Role
        private static async Task AddClaimsToManagerRole(RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            try
            {
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Select"));
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Add"));
                await AddClaimIfNotExists(roleManager, role, new Claim("Permission", "Update"));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while adding claims to the Manager role.");
                throw;
            }
        }

    }
}
