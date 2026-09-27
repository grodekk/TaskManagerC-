using Microsoft.AspNetCore.Identity;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Employee", "Viewer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Could not create role: {role}");
            }
        }

        var adminUsername = configuration["Admin:Username"];
        var adminPassword = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(adminUsername) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            return;
        }

        var admin = await userManager.FindByNameAsync(adminUsername);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminUsername
            };

            var createResult = await userManager.CreateAsync(
                admin,
                adminPassword);

            if (!createResult.Succeeded)
                throw new InvalidOperationException(
                    "Could not create initial admin user.");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                admin,
                "Admin");

            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    "Could not assign Admin role.");
        }
    }
}