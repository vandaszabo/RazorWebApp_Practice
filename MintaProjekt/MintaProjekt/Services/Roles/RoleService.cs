using Microsoft.AspNetCore.Identity;
using MintaProjekt.Models;
using System.Security.Claims;

namespace MintaProjekt.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }


        // Create Role
        public async Task<IdentityRole> CreateRole(string roleName)
        {
            _logger.LogDebug("Start CreateRole method.");
            try
            {
                var roleExist = await _roleRepository.RoleExistsAsync(roleName);

                // Create role if not exist
                if (!roleExist)
                {
                    var result = await _roleRepository.CreateRoleAsync(roleName);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Role {RoleName} created successfully", roleName);
                    }
                    else
                    {
                        _logger.LogError("Error occurred while creating role {roleName}: ", roleName);
                        throw new InvalidOperationException("Error occured in RoleService - CreateRole method while creating a role.");
                    }
                }
                // Find the previously created role, return it
                var role = await _roleRepository.FindRoleByNameAsync(roleName);
                if (role == null)
                {
                    _logger.LogError($"Role {roleName} was not found after creation.");
                    throw new InvalidOperationException("Exeption occured in RoleService - CreateRole method while retrieving previously created role.");
                }

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating or retrieving the role {RoleName}", roleName);
                throw new InvalidOperationException($"Failed to create role {roleName}");
            }
        }

        // Add List of claims to a role
        public async Task AddClaimsToRole(IdentityRole role, List<Claim> claims)
        {
            _logger.LogDebug("Start AddClaimsToRole method.");
            try
            {
                foreach (var claim in claims)
                {
                    await AddClaimIfNotExists(role, claim);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding claims to the User role.");
                throw;
            }
        }

        // Assign new claim to a role
        public async Task AddClaimIfNotExists(IdentityRole role, Claim claim)
        {
            _logger.LogDebug("Start AddClaimIfNotExists method.");
            var claims = await _roleRepository.GetClaimsAsync(role);

            // Add new claim if it doesn't exist
            if (!claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                var result = await _roleRepository.AddClaimAsync(role, claim);
                if (!result.Succeeded)
                {
                    _logger.LogError("Error occured while adding claim to role: {roleName} - {claimType}, {claimValue}", role.Name, claim.Type, claim.Value);
                    throw new InvalidOperationException("Error occured while adding claim to role.");
                }
                _logger.LogInformation("Claim added successfully. {roleName} - {claimType}, {claimValue}", role.Name, claim.Type, claim.Value);
            }
        }


        // Get claims for specific role
        public async Task<IEnumerable<Claim>> GetClaimsForRoleAsync(string roleID)
        {
            _logger.LogDebug("Start GetClaimsForRoleAsync method.");
            try
            {
                // Get the role object
                var role = await _roleRepository.FindRoleByIdAsync(roleID);
                if (role == null)
                {
                    _logger.LogWarning("Cannot find role: {roleID}", roleID);
                    throw new ArgumentException("Cannot find role with {roleID} ", roleID);
                }

                // Get its claims
                var roleClaims = await _roleRepository.GetClaimsAsync(role);
                return roleClaims;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in RoleService - GetClaimsForRoleAsync method.");
                throw;
            }
        }


        // Get all roles
        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            _logger.LogDebug("Start GetAllRoles method.");
            try
            {
                var roles= await _roleRepository.GetAllRolesAsync();
                if(!roles.Any())
                {
                    _logger.LogWarning("No roles found.");
                }

                _logger.LogDebug("Roles found successfully in RoleService- GetAllRoles method.");
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in RoleService - GetAllRoles.");
                throw;
            }
        }

        // Get Roles with their claims
        public async Task<IEnumerable<RoleWithClaims>> GetRolesWithClaims()
        {
            _logger.LogDebug("Start GetRolesWithClaims method.");
            try
            {
                var roles = await GetAllRoles();

                var rolesWithClaims = new List<RoleWithClaims>();
                foreach (var role in roles)
                {
                    var claims = await GetClaimsForRoleAsync(role.Id);
                    rolesWithClaims.Add(new RoleWithClaims(role, claims));
                }

                return rolesWithClaims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exeption occured in RoleService GetRolesWithClaims method.");
                throw new InvalidDataException(); // TODO custom UI exeption
            }
        }
    }
}
