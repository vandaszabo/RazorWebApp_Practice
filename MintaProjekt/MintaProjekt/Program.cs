using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MintaProjekt.DbContext;
using MintaProjekt.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;
using MintaProjekt.Authorization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using MintaProjekt.Utilities;
using Microsoft.AspNetCore.Mvc.Razor;

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
            RoleInitializer.AddRoles(app);

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
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Munkamenet idõkorlát (idle = tétlen)
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            options => builder.Configuration.Bind("CookieSettings", options));
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
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false) // Set true if confirmation required
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<UserDbContext>();

        }

    }
}
