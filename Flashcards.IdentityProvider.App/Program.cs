using Duende.IdentityServer.Licensing;
using Flashcards.Common.Extensions;
using Flashcards.IdentityProvider.App;
using Flashcards.IdentityProvider.App.Endpoints;
using Flashcards.IdentityProvider.App.Installers;
using Flashcards.IdentityProvider.BL.Installers;
using Flashcards.IdentityProvider.DAL.Installers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Flashcards.IdentityProvider.DAL.Entities;
using Serilog;
using System.Globalization;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");


try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddInstaller<IdentityProviderDALInstaller>();
    builder.Services.AddInstaller<IdentityProviderBLInstaller>();
    builder.Services.AddInstaller<IdentityProviderAppInstaller>();

    // TODO: this should be in Api.App
    builder.Services.AddAuthorizationBuilder();

    var app = builder
        .ConfigureLogging()
        .ConfigureServices()
        .ConfigurePipeline();

    if (app.Environment.IsDevelopment())
    {
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            var usage = app.Services.GetRequiredService<LicenseUsageSummary>();
            Console.Write(Summary(usage));
        });
    }

    app.MapGroup("api")
        .AllowAnonymous()
        .UseUserEndpoints();

    //////
    /*using (var scope = app.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserEntity>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRoleEntity>>();

        // Create roles if not exists
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new AppRoleEntity(){ Name = "Admin"});

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new AppRoleEntity(){ Name = "User"});

        // Create admin user if not exists
        string username = "User";
        string adminEmail = "User@email.com";
        string adminPassword = "User123";

        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)
        {
            var admin = new AppUserEntity
            {
                UserName = username,
                Email = adminEmail,
                EmailConfirmed = true,
                Active = true,
                Subject = username
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "User");
                Console.WriteLine("User user created successfully!");
            }
            else
            {
                Console.WriteLine("Failed to create admin user:");
                foreach (var e in result.Errors)
                    Console.WriteLine($" - {e.Description}");
            }
        }
    }*/
    //as

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static string Summary(LicenseUsageSummary usage)
{
    var sb = new StringBuilder();
    sb.AppendLine("IdentityServer Usage Summary:");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  License: {usage.LicenseEdition}");
    var features = usage.FeaturesUsed.Count > 0 ? string.Join(", ", usage.FeaturesUsed) : "None";
    sb.AppendLine(CultureInfo.InvariantCulture, $"  Business and Enterprise Edition Features Used: {features}");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.ClientsUsed.Count} Client Id(s) Used");
    sb.AppendLine(CultureInfo.InvariantCulture, $"  {usage.IssuersUsed.Count} Issuer(s) Used");

    return sb.ToString();
}
