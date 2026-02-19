using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Watchlog.Business.Options;
using Watchlog.Business.Repositories.Implementations;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Implementations;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Data.Persistance;

namespace WatchLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ITmdbService, TmdbService>();

            builder.Services.AddHttpClient("tmdb", client =>
            {
                client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
            });

            builder.Services.Configure<TmdbOptions>(builder.Configuration.GetSection("Tmdb"));
            builder.Services.AddScoped<ITmdbService, TmdbService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Titles}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}