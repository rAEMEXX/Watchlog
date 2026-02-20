using Microsoft.EntityFrameworkCore;
using Watchlog.Data.Persistance;
using Watchlog.Models.Domain.Entities;

namespace Watchlog.Data.Seed;

public static class TitleSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Prevent duplicate seeding
        if (await db.Titles.AnyAsync())
            return;

        db.Titles.AddRange(
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

        await db.SaveChangesAsync();
    }
}