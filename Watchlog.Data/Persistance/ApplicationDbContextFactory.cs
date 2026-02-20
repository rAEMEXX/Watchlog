using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Watchlog.Data.Persistance
{
    /// <summary>
    /// Provides a design-time factory for creating <see cref="ApplicationDbContext"/> instances
    /// used by Entity Framework Core tools.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDbContext"/> for design-time services
        /// such as migrations and database updates.
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        /// <returns>A configured <see cref="ApplicationDbContext"/> instance</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Uses the same connection string settings as the Web project
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=WatchlogDb;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}