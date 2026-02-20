using Microsoft.EntityFrameworkCore;
using Watchlog.Data.Persistance;
using Watchlog.Models.Domain.Entities;

namespace Watchlog.Data.Seed
{
    /// <summary>
    /// Seeds demo titles
    /// </summary>
    public static class TitleSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            // Prevent duplicate seeding
            if (await dbContext.Titles.AnyAsync())
                return;

            dbContext.Titles.AddRange(
                new Title
                {
                    TmdbId = 62560,
                    Name = "Mr. Robot",
                    IsSeries = true,
                    ReleaseYear = 2015,
                    Overview = "A cybersecurity engineer and hacker is recruited by a mysterious anarchist."
                },
                new Title
                {
                    TmdbId = 155,
                    Name = "The Dark Knight",
                    IsSeries = false,
                    ReleaseYear = 2008,
                    Overview = "Batman faces the Joker, a criminal mastermind spreading chaos in Gotham."
                }
            );

            await dbContext.SaveChangesAsync();
        }
    }
}