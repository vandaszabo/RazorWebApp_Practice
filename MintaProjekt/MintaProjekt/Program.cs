using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MintaProjekt.DbContext;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using MintaProjekt.Utilities;
using Microsoft.AspNetCore.Mvc.Razor;
using MintaProjekt.Services.Departments;
using MintaProjekt.Services.Employees;
using MintaProjekt.Services.Roles;
using MintaProjekt.Services.Users;
using System.Security.Claims;
using Microsoft.Extensions.Options;

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

            // Add Pages with localization
            builder.Services.AddRazorPages()
                       .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                       .AddDataAnnotationsLocalization();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("hu-HU")
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });
            // Add Session
            AddSession(builder);

            AddControllers(builder);
            AddScopedServices(builder);
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Initialize Roles
            InitRoles(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Session Middleware => HttpContext.Session is available for use
            app.UseSession();

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

        // Add Session
        private static void AddSession(WebApplicationBuilder builder)
        {
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Munkamenet id�korl�t (idle = t�tlen)
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // the cookie is essential for the application�s basic functionality
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

            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<IUserTransactions, UserTransactions>();

            builder.Services.AddScoped<UserHelper>();
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
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // Bind options from configuration
                builder.Configuration.Bind("CookieSettings", options);
                // Configure the event to validate the security stamp
                options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
            });
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero; // Validate on every request
            });
        }

        // Add Authorization
        private static void AddAuthorization(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("CanSelectData", policy => policy.RequireClaim("Permission", "Select"))
                .AddPolicy("CanInsertData", policy => policy.RequireClaim("Permission", "Insert"))
                .AddPolicy("CanUpdateData", policy => policy.RequireClaim("Permission", "Update"))
                .AddPolicy("CanDeleteData", policy => policy.RequireClaim("Permission", "Delete"));
        }

        // Add Identity
        private static void AddIdentity(WebApplicationBuilder builder)
        {
            builder.Services.AddDefaultIdentity<IdentityUser>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<UserDbContext>();

        }

        // Initialize 3 Roles: Admin, User, Manager
        public static void InitRoles(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

            try
            {
                // Admin
                var adminRole = roleService.CreateRole("Admin").Result;
                List<Claim> adminClaims = new()
                {
                new Claim("Permission", "Select"),
                new Claim("Permission", "Insert"),
                new Claim("Permission", "Update"),
                new Claim("Permission", "Delete")
                };
                roleService.AddClaimsToRole(adminRole, adminClaims).Wait();

                // User
                var userRole = roleService.CreateRole("User").Result;
                List<Claim> userClaims = new()
                {
                new Claim("Permission", "Select")
                };
                roleService.AddClaimsToRole(userRole, userClaims).Wait();

                // Manager
                var managerRole = roleService.CreateRole("Manager").Result;
                List<Claim> managerClaims = new()
                {
                new Claim("Permission", "Select"),
                new Claim("Permission", "Insert"),
                new Claim("Permission", "Update")
                };
                roleService.AddClaimsToRole(managerRole, managerClaims).Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An error occurred while creating or setting up roles.");
            }
        }

    }
}
