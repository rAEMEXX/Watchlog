using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WatchLog.Models.Domain.Entities;

namespace WatchLog.Data.Persistance
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// ===== Catalog tables =====
		public DbSet<Title> Titles { get; set; } = null!;
		public DbSet<Genre> Genres { get; set; } = null!;
		public DbSet<Season> Seasons { get; set; } = null!;
		public DbSet<Episode> Episodes { get; set; } = null!;
		public DbSet<TitleGenre> TitleGenres { get; set; } = null!;

        public DbSet<UserTitle> UserTitles { get; set; } = null!;

        public DbSet<UserTitleProgress> UserTitleProgresses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TitleGenre>()
                .HasKey(tg => new { tg.TitleId, tg.GenreId });

            // (ако имаш UserTitle уникалност)
            builder.Entity<UserTitle>()
                .HasIndex(x => new { x.UserId, x.TitleId })
                .IsUnique();
        }
    }
}