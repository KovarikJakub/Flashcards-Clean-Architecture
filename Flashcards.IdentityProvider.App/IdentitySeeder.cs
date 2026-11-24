using Flashcards.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.IdentityProvider.App
{
    internal static class IdentitySeeder
    {
        // Must match the role names the API sends (DuendeIdentityService.CreateIdentityAsync).
        private static readonly string[] Roles = ["Admin", "User"];

        /// <summary>
        /// Ensures the application roles exist and assigns the Admin role to the users
        /// listed in configuration under "Seed:AdminUserNames" (only those that already exist).
        /// </summary>
        public static async Task SeedIdentityAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRoleEntity>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserEntity>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentitySeeder));

            foreach (var roleName in Roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new AppRoleEntity { Id = Guid.NewGuid(), Name = roleName });
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role {Role}", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            var adminUserNames = app.Configuration.GetSection("Seed:AdminUserNames").Get<string[]>() ?? [];
            foreach (var userName in adminUserNames)
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user is null)
                {
                    logger.LogWarning("Seed admin user {UserName} does not exist yet; register it and restart to grant Admin", userName);
                    continue;
                }

                if (await userManager.IsInRoleAsync(user, "Admin"))
                {
                    continue;
                }

                var result = await userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    logger.LogInformation("Granted Admin role to {UserName}", userName);
                }
                else
                {
                    logger.LogError("Failed to grant Admin role to {UserName}: {Errors}", userName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
