using Microsoft.EntityFrameworkCore;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.Tmdb;
using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Implementations
{
    public class CatalogImportService : ICatalogImportService
    {
        private readonly ITmdbService _tmdb;
        private readonly IRepository<Title> _titles;
        private readonly IRepository<Genre> _genres;
        private readonly IRepository<UserTitle> _userTitles;

        public CatalogImportService(
            ITmdbService tmdb,
            IRepository<Title> titles,
            IRepository<Genre> genres,
            IRepository<UserTitle> userTitles)
        {
            _tmdb = tmdb;
            _titles = titles;
            _genres = genres;
            _userTitles = userTitles;
        }

        public async Task<ImportResultsViewModel> SearchAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new InvalidOperationException("Please enter a movie or series name.");

            var tvResults = await _tmdb.SearchTvAsync(query);
            var movieResults = await _tmdb.SearchMovieAsync(query);

            return new ImportResultsViewModel
            {
                Query = query,
                TvResults = tvResults?.Results ?? new List<TmdbSearchItem>(),
                MovieResults = movieResults?.Results ?? new List<TmdbSearchItem>()
            };
        }

        public async Task<string> ImportSelectedAsync(string userId, int tmdbId, string type, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            if (type != "tv" && type != "movie")
                throw new InvalidOperationException("Invalid type.");

            // 1) global title exists?
            var title = await _titles.Query()
                .Include(t => t.Seasons)
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId, ct);

            // 2) if not, import from TMDB
            if (title == null)
            {
                if (type == "tv")
                {
                    var tv = await _tmdb.GetTvDetailsAsync(tmdbId) ?? throw new InvalidOperationException("TMDB TV title not found.");

                    title = new Title
                    {
                        TmdbId = tv.Id,
                        Name = tv.Name,
                        IsSeries = true,
                        ReleaseYear = ParseYear(tv.FirstAirDate),
                        Overview = tv.Overview
                    };

                    await AddGenresAsync(title, tv.Genres, ct);

                    foreach (var s in tv.Seasons.Where(x => x.SeasonNumber > 0))
                    {
                        title.Seasons.Add(new Season
                        {
                            SeasonNumber = s.SeasonNumber,
                            EpisodeCount = s.EpisodeCount
                        });
                    }

                    await _titles.AddAsync(title, ct);
                    await _titles.SaveChangesAsync(ct);
                }
                else
                {
                    var movie = await _tmdb.GetMovieDetailsAsync(tmdbId) ?? throw new InvalidOperationException("TMDB movie not found.");

                    title = new Title
                    {
                        TmdbId = movie.Id,
                        Name = movie.Title,
                        IsSeries = false,
                        ReleaseYear = ParseYear(movie.ReleaseDate),
                        Overview = movie.Overview
                    };

                    await AddGenresAsync(title, movie.Genres, ct);

                    await _titles.AddAsync(title, ct);
                    await _titles.SaveChangesAsync(ct);
                }
            }

            // 3) link to user catalog (UserTitles) if missing
            var alreadyInCatalog = await _userTitles.Query()
                .AsNoTracking()
                .AnyAsync(ut => ut.UserId == userId && ut.TitleId == title.Id, ct);

            if (!alreadyInCatalog)
            {
                await _userTitles.AddAsync(new UserTitle
                {
                    UserId = userId,
                    TitleId = title.Id
                }, ct);

                await _userTitles.SaveChangesAsync(ct);
            }

            return title.Name;
        }

        private static int? ParseYear(string? date)
        {
            if (string.IsNullOrWhiteSpace(date) || date.Length < 4)
                return null;

            return int.TryParse(date.Substring(0, 4), out var y) ? y : null;
        }

        private async Task AddGenresAsync(Title title, List<TmdbGenre> tmdbGenres, CancellationToken ct)
        {
            foreach (var g in tmdbGenres)
            {
                var existing = await _genres.Query()
                    .FirstOrDefaultAsync(x => x.TmdbId == g.Id, ct);

                if (existing == null)
                {
                    existing = new Genre
                    {
                        TmdbId = g.Id,
                        Name = g.Name
                    };

                    await _genres.AddAsync(existing, ct);
                    // IMPORTANT: no SaveChanges yet; we can batch-save later
                }

                title.TitleGenres.Add(new TitleGenre
                {
                    Genre = existing
                });
            }

            // Ensure new genres are persisted before Title save can reference them
            await _genres.SaveChangesAsync(ct);
        }
    }
}