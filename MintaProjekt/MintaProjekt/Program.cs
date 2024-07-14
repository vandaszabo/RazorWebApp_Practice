using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MintaProjekt.DbContext;
using MintaProjekt.Services;
using Serilog;

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
        builder.Services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("MSSQLConnection")));

        builder.Services.AddRazorPages();
        builder.Services.AddScoped<EmployeeDataService>();  // Register Service Class
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