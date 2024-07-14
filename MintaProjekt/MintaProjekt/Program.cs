using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using MintaProjekt.DbContext;
using MintaProjekt.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Serilog configuration
        var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();


        // Use Serilog
        builder.Host.UseSerilog();

        // Add services to the container.

        // Add authentication services and handlers for cookie
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options));

        // Add DbContext
        builder.Services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("MSSQLConnection")));


        builder.Services.AddRazorPages();

        // Using an authorization filter, setting the fallback policy uses endpoint routing. Require all users be authenticated.
        builder.Services.AddControllers(config =>
        {
            var policy = new AuthorizationPolicyBuilder()
                             .RequireAuthenticatedUser()
                             .Build();
            config.Filters.Add(new AuthorizeFilter(policy));
        });

        // Register Service Classes
        builder.Services.AddScoped<EmployeeDataService>();  
        builder.Services.AddScoped<DepartmentDataService>();


        var app = builder.Build();

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
}