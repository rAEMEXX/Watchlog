using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Watchlog.Business.Authorization.Handlers;
using Watchlog.Business.Authorization.Requirements;
using Watchlog.Business.Options;
using Watchlog.Business.Repositories.Implementations;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Implementations;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Data.Persistance;
using Watchlog.Data.Seed;
using Watchlog.Business.Authorization;

namespace WatchLog;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // ✅ Identity + Roles (Foodie-style)
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // ✅ Repository (keep one lifetime consistently)
        builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

        // ✅ Your services
        builder.Services.AddTransient<ICatalogImportService, CatalogImportService>();
        builder.Services.AddTransient<IUserCatalogService, UserCatalogService>();
        builder.Services.AddTransient<IUserProgressService, UserProgressService>();

        // ✅ TMDB HttpClient + Options + service (ONLY ONCE)
        builder.Services.AddHttpClient("tmdb", client =>
        {
            client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        });

        builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));
        builder.Services.AddTransient<ITmdbService, TmdbService>();

        // ✅ Authorization policy + handler (Foodie-style)
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.UserTitleAccessPolicy, policy =>
            {
                policy.Requirements.Add(new UserTitleAccessRequirement());
            });

        builder.Services.AddScoped<IAuthorizationHandler, UserTitleAccessHandler>();

        var app = builder.Build();

        // ✅ Seed data (Foodie-style)
        using (var scope = app.Services.CreateScope())
        {
            await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // ✅ REQUIRED: Authentication before Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Titles}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.Run();
    }
}