using AutoMapper;
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
        private readonly IMapper _mapper;

        public CatalogImportService(
            ITmdbService tmdb,
            IRepository<Title> titles,
            IRepository<Genre> genres,
            IRepository<UserTitle> userTitles,
            IMapper mapper)
        {
            _tmdb = tmdb;
            _titles = titles;
            _genres = genres;
            _userTitles = userTitles;
            _mapper = mapper;
        }

        public async Task<ImportResultsViewModel> SearchAsync(string query, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new InvalidOperationException("Search query is required.");

            var tvTask = _tmdb.SearchTvAsync(query);
            var movieTask = _tmdb.SearchMovieAsync(query);

            await Task.WhenAll(tvTask, movieTask);

            var tv = tvTask.Result;
            var movie = movieTask.Result;

            return new ImportResultsViewModel
            {
                Query = query,
                TvResults = tv?.Results ?? new(),
                MovieResults = movie?.Results ?? new()
            };
        }

        public async Task<string> ImportSelectedAsync(string userId, int tmdbId, string type, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            var isSeries = type.Equals("tv", StringComparison.OrdinalIgnoreCase)
                        || type.Equals("series", StringComparison.OrdinalIgnoreCase);

            var title = await _titles.Query()
                .Include(t => t.Seasons)
                .Include(t => t.TitleGenres)
                    .ThenInclude(tg => tg.Genre)
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId && t.IsSeries == isSeries, ct);

            if (title == null)
            {
                title = await CreateTitleFromTmdbAsync(tmdbId, isSeries, ct);

                await _titles.AddAsync(title, ct);
                await _titles.SaveChangesAsync(ct);
            }

            // Ensure user link exists
            var exists = await _userTitles.Query()
                .AsNoTracking()
                .AnyAsync(ut => ut.UserId == userId && ut.TitleId == title.Id, ct);

            if (!exists)
            {
                await _userTitles.AddAsync(new UserTitle
                {
                    UserId = userId,
                    TitleId = title.Id,
                    AddedAt = DateTime.UtcNow
                }, ct);

                await _userTitles.SaveChangesAsync(ct);
            }

            return title.Name;
        }

        private async Task<Title> CreateTitleFromTmdbAsync(int tmdbId, bool isSeries, CancellationToken ct)
        {
            if (isSeries)
            {
                var tv = await _tmdb.GetTvDetailsAsync(tmdbId)
                         ?? throw new InvalidOperationException("TMDB TV title not found.");

                var title = _mapper.Map<Title>(tv);

                // map seasons (skip season 0 / specials)
                title.Seasons = tv.Seasons
                    .Where(s => s.SeasonNumber > 0)
                    .Select(s => _mapper.Map<Season>(s))
                    .ToList();

                await AttachGenresAsync(title, tv.Genres, ct);

                return title;
            }
            else
            {
                var movie = await _tmdb.GetMovieDetailsAsync(tmdbId)
                            ?? throw new InvalidOperationException("TMDB movie not found.");

                var title = _mapper.Map<Title>(movie);

                // movies have no seasons in your domain model
                title.Seasons = new List<Season>();

                await AttachGenresAsync(title, movie.Genres, ct);

                return title;
            }
        }

        private async Task AttachGenresAsync(Title title, List<TmdbGenre> tmdbGenres, CancellationToken ct)
        {
            if (tmdbGenres == null || tmdbGenres.Count == 0)
                return;

            // Load all existing genres by tmdb ids
            var ids = tmdbGenres.Select(g => g.Id).ToList();

            var existing = await _genres.Query()
                .Where(g => ids.Contains(g.TmdbId))
                .ToListAsync(ct);

            foreach (var tg in tmdbGenres)
            {
                var genre = existing.FirstOrDefault(g => g.TmdbId == tg.Id);
                if (genre == null)
                {
                    genre = new Genre
                    {
                        TmdbId = tg.Id,
                        Name = tg.Name
                    };

                    await _genres.AddAsync(genre, ct);
                    existing.Add(genre);
                }

                // prevent duplicates inside TitleGenres
                if (!title.TitleGenres.Any(x => x.Genre != null && x.Genre.TmdbId == tg.Id))
                {
                    title.TitleGenres.Add(new TitleGenre
                    {
                        Title = title,
                        Genre = genre
                    });
                }
            }

            // Save new genres (Title is saved by caller)
            await _genres.SaveChangesAsync(ct);
        }
    }
}