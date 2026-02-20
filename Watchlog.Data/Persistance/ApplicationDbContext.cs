using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Watchlog.Models.Domain.Entities;

namespace Watchlog.Data.Persistance
{
    /// <summary>
    /// Represents the database context for the Watchlog application
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ===== Catalog tables =====

        /// <summary>
        /// Gets or sets the titles table database set
        /// </summary>
        public DbSet<Title> Titles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the genres table database set
        /// </summary>
        public DbSet<Genre> Genres { get; set; } = null!;

        /// <summary>
        /// Gets or sets the seasons table database set
        /// </summary>
        public DbSet<Season> Seasons { get; set; } = null!;

        /// <summary>
        /// Gets or sets the episodes table database set
        /// </summary>
        public DbSet<Episode> Episodes { get; set; } = null!;

        /// <summary>
        /// Gets or sets the title-genre join table database set
        /// </summary>
        public DbSet<TitleGenre> TitleGenres { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user titles table database set
        /// </summary>
        public DbSet<UserTitle> UserTitles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user title progress table database set
        /// </summary>
        public DbSet<UserTitleProgress> UserTitleProgresses { get; set; } = null!;

        /// <summary>
        /// Configures the entity relationships and constraints for the database model
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Title <-> Genre (many-to-many)
            builder.Entity<TitleGenre>()
                .HasKey(tg => new { tg.TitleId, tg.GenreId });

            // Ensure a user cannot add the same title twice
            builder.Entity<UserTitle>()
                .HasIndex(x => new { x.UserId, x.TitleId })
                .IsUnique();
        }
    }
}