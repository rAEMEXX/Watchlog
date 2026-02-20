using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Watchlog.Data.Seed
{
    /// <summary>
    /// Seeds roles and default users
    /// </summary>
    public static class UserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            RoleManager<IdentityRole> roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            UserManager<IdentityUser> userManager =
                services.GetRequiredService<UserManager<IdentityUser>>();

            // Create required roles
            await EnsureRole(roleManager, "Admin");
            await EnsureRole(roleManager, "User");

            // Create default admin user (used for admin access)
            await EnsureUser(
                userManager,
                email: "admin@admin.com",
                password: "Admin#123",
                role: "Admin");
        }

        private static async Task EnsureRole(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        private static async Task EnsureUser(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var created = await userManager.CreateAsync(user, password);

                if (!created.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create seeded user {email}: " +
                        $"{string.Join(", ", created.Errors.Select(e => e.Description))}");
            }

            // Ensure role assignment
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
    }
}