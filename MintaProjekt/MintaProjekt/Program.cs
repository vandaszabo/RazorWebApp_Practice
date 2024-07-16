using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MintaProjekt.DbContext;
using MintaProjekt.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace MintaProjekt
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            AddSerilog(builder);
            AddDbContext(builder, configuration);
            AddAuthentication(builder);
            AddAuthorization(builder);
            AddIdentity(builder);
            AddControllers(builder);
            AddScopedServices(builder);

            var app = builder.Build();

            AddAdminRole(app);
            AddUserRole(app);
            AddManagerRole(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            try
            {
                Log.Information("Starting up the host");
                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // Add Controllers
        private static void AddControllers(WebApplicationBuilder builder)
        {
            // Add Controllers With Authorization filter
            //Require all users to be authenticated.
            builder.Services.AddControllers(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // Add Serilog
        private static void AddSerilog(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();
        }

        // Add Scoped Services
        private static void AddScopedServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IEmployeeDataService, EmployeeDataService>();
            builder.Services.AddScoped<IDepartmentDataService, DepartmentDataService>();
        }

        // Add DbContext
        private static void AddDbContext(WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MSSQLConnection")));
        }

        // Add Authentication
        private static void AddAuthentication(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            options => builder.Configuration.Bind("CookieSettings", options));
        }

        // Add Authorization
        private static void AddAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanSelectData", policy => policy.RequireClaim("Permission", "Select"));
                options.AddPolicy("CanAddData", policy => policy.RequireClaim("Permission", "Add"));
                options.AddPolicy("CanUpdateData", policy => policy.RequireClaim("Permission", "Update"));
                options.AddPolicy("CanDeleteData", policy => policy.RequireClaim("Permission", "Delete"));
            });
        }

        // Add Identity
        private static void AddIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) // Set true if confirmation required
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<UserDbContext>();

            builder.Services.AddRazorPages();
        }

        // Add Admin Role
        private static void AddAdminRole(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                var adminRole = CreateRole(roleManager, "Admin").Result;
                AddClaimsToAdminRole(roleManager, adminRole).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or setting up the Admin role.");
            }
        }

        // Add User Role
        private static void AddUserRole(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                var userRole = CreateRole(roleManager, "User").Result;
                AddClaimsToUserRole(roleManager, userRole).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or setting up the User role.");
            }
        }

        // Add Manager Role
        private static void AddManagerRole(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try
            {
                var userRole = CreateRole(roleManager, "Manager").Result;
                AddClaimsToManagerRole(roleManager, userRole).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or setting up the Manager role.");
            }
        }


        // Create Roles
        private static async Task<IdentityRole> CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            try
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                // ha nincs még ilyen role, létrehozzuk
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
                // megkeressük és megadjuk visszatérési értéknek, ha nem találjuk hibát dobunk
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


        // Create Claims for Admin Role
        private static async Task AddClaimsToAdminRole(RoleManager<IdentityRole> roleManager, IdentityRole role)
        {
            try
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Select"));
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Add"));
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Update"));
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Delete"));
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
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Select"));
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
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Select")); //type and value
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Add"));
                await roleManager.AddClaimAsync(role, new Claim("Permission", "Update"));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while adding claims to the Manager role.");
                throw;
            }
        }

    }
}
