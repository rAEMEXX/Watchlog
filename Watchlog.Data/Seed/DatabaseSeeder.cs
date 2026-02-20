using Microsoft.Extensions.DependencyInjection;
using Watchlog.Data.Persistance;

namespace Watchlog.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();

        // 1) Seed roles/users first (depends on Identity services)
        await UserSeeder.SeedAsync(sp);

        // 2) Seed minimal app data
        await TitleSeeder.SeedAsync(db);
    }
}