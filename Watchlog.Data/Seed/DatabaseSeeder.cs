using Microsoft.Extensions.DependencyInjection;
using Watchlog.Data.Persistance;

namespace Watchlog.Data.Seed
{
    /// <summary>
    /// Seeds demo data
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using IServiceScope scope = services.CreateScope();
            IServiceProvider scopeProvider = scope.ServiceProvider;

            ApplicationDbContext dbContext = scopeProvider.GetRequiredService<ApplicationDbContext>();

            // 1) Seed roles and users (requires Identity services)
            await UserSeeder.SeedAsync(scopeProvider);

            // 2) Seed minimal application data
            await TitleSeeder.SeedAsync(dbContext);
        }
    }
}