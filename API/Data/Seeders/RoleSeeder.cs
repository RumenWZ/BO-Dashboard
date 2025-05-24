using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API.Data.Seeders
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var logger = services.GetRequiredService<ILogger<RoleSeeder>>();

            string[] roles = new[] { "Admin", "User", "Employee" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} role"
                    };

                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{roleName}' created successfully.");
                    }
                    else
                    {
                        logger.LogWarning($"Failed to create role '{roleName}': {string.Join(", ", result.Errors)}");
                    }
                }
            }
        }
    }
}
