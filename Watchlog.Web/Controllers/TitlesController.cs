using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watchlog.Data.Persistance;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.ViewModels;
using Watchlog.Models.Tmdb;
using Watchlog.Business.Services.Implementations;
using Watchlog.Business.Services.Interfaces;

namespace Watchlog.Controllers
{
    [Authorize]
    public class TitlesController : Controller
    {
        private readonly ITmdbService _tmdbService;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public TitlesController(
            ApplicationDbContext db,
            ITmdbService tmdbService,
            UserManager<IdentityUser> userManager)
        {
            _db = db;
            _tmdbService = tmdbService;
            _userManager = userManager;
        }

        // ================= IMPORT =================

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError("", "Please enter a movie or series name.");
                return View();
            }

            var tvResults = await _tmdbService.SearchTvAsync(query);
            var movieResults = await _tmdbService.SearchMovieAsync(query);

            var model = new ImportResultsViewModel
            {
                Query = query,
                TvResults = tvResults?.Results ?? new List<TmdbSearchItem>(),
                MovieResults = movieResults?.Results ?? new List<TmdbSearchItem>()
            };

            return View("ImportResults", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSelected(int tmdbId, string type)
        {
            if (type != "tv" && type != "movie")
                return BadRequest();

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // 1) намери дали заглавието вече съществува глобално
            var title = await _db.Titles
                .Include(t => t.Seasons)
                .FirstOrDefaultAsync(t => t.TmdbId == tmdbId);

            // 2) ако НЕ съществува, импортни го от TMDB
            if (title == null)
            {
                if (type == "tv")
                {
                    var tv = await _tmdbService.GetTvDetailsAsync(tmdbId);
                    if (tv == null) return NotFound();

                    title = new Title
                    {
                        TmdbId = tv.Id,
                        Name = tv.Name,
                        IsSeries = true,
                        ReleaseYear = ParseYear(tv.FirstAirDate),
                        Overview = tv.Overview
                    };

                    await AddGenresAsync(title, tv.Genres);

                    foreach (var s in tv.Seasons.Where(x => x.SeasonNumber > 0))
                    {
                        title.Seasons.Add(new Season
                        {
                            SeasonNumber = s.SeasonNumber,
                            EpisodeCount = s.EpisodeCount
                        });
                    }

                    _db.Titles.Add(title);
                    await _db.SaveChangesAsync();
                }
                else // movie
                {
                    var movie = await _tmdbService.GetMovieDetailsAsync(tmdbId);
                    if (movie == null) return NotFound();

                    title = new Title
                    {
                        TmdbId = movie.Id,
                        Name = movie.Title,
                        IsSeries = false,
                        ReleaseYear = ParseYear(movie.ReleaseDate),
                        Overview = movie.Overview
                    };

                    await AddGenresAsync(title, movie.Genres);

                    _db.Titles.Add(title);
                    await _db.SaveChangesAsync();
                }
            }

            // 3) добави заглавието към ЛИЧНИЯ каталог (UserTitles), ако го няма
            var alreadyInCatalog = await _db.UserTitles
                .AnyAsync(ut => ut.UserId == userId && ut.TitleId == title.Id);

            if (!alreadyInCatalog)
            {
                _db.UserTitles.Add(new UserTitle
                {
                    UserId = userId,
                    TitleId = title.Id
                });

                await _db.SaveChangesAsync();
            }

            TempData["SavedMsg"] = $"Added \"{title.Name}\" to your catalog.";
            return RedirectToAction(nameof(Index));
        }

        // ================= CATALOG (PER USER) =================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // само заглавията, които са в UserTitles за този user
            var titles = await _db.UserTitles
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Title)
                    .ThenInclude(t => t.Seasons)
                .OrderByDescending(ut => ut.AddedAt)
                .Select(ut => ut.Title)
                .ToListAsync();

            var progressByTitleId = await _db.UserTitleProgresses
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.TitleId);

            var model = titles.Select(t =>
            {
                progressByTitleId.TryGetValue(t.Id, out var p);

                return new TitleCatalogItemViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    ReleaseYear = t.ReleaseYear,
                    IsSeries = t.IsSeries,

                    Seasons = t.Seasons
                        .OrderBy(s => s.SeasonNumber)
                        .Select(s => new SeasonOption
                        {
                            SeasonNumber = s.SeasonNumber,
                            EpisodeCount = s.EpisodeCount
                        })
                        .ToList(),

                    CurrentSeason = p?.CurrentSeason,
                    CurrentEpisode = p?.CurrentEpisode,
                    Status = p?.Status
                };
            }).ToList();

            return View(model);
        }

        // маха само от личния каталог (НЕ трие от Titles)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCatalog(int titleId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var link = await _db.UserTitles
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TitleId == titleId);

            if (link != null)
            {
                _db.UserTitles.Remove(link);

                // по желание: махни и прогреса за този title за този user
                var progress = await _db.UserTitleProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId);

                if (progress != null)
                    _db.UserTitleProgresses.Remove(progress);

                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= PROGRESS =================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int titleId, int season, int? episode)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // защита: не позволявай да update-ваш заглавие, което не е в твоя каталог
            var ownsTitle = await _db.UserTitles.AnyAsync(ut => ut.UserId == userId && ut.TitleId == titleId);
            if (!ownsTitle) return Forbid();

            var epValue = episode ?? 1;
            if (season < 1) season = 1;
            if (epValue < 1) epValue = 1;

            // clamp към max епизоди в сезона
            var seasonRow = await _db.Seasons
                .Where(s => s.TitleId == titleId && s.SeasonNumber == season)
                .Select(s => new { s.EpisodeCount })
                .FirstOrDefaultAsync();

            if (seasonRow != null && seasonRow.EpisodeCount > 0 && epValue > seasonRow.EpisodeCount)
                epValue = seasonRow.EpisodeCount;

            var progress = await _db.UserTitleProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId);

            if (progress == null)
            {
                progress = new UserTitleProgress
                {
                    UserId = userId,
                    TitleId = titleId
                };
                _db.UserTitleProgresses.Add(progress);
            }

            progress.CurrentSeason = season;
            progress.CurrentEpisode = epValue;

            // completed ако е последен сезон + последен епизод
            var lastSeason = await _db.Seasons
                .Where(s => s.TitleId == titleId)
                .OrderByDescending(s => s.SeasonNumber)
                .FirstOrDefaultAsync();

            if (lastSeason != null && season == lastSeason.SeasonNumber && epValue == lastSeason.EpisodeCount)
                progress.Status = "Completed";
            else
                progress.Status = "Watching";

            await _db.SaveChangesAsync();

            var titleName = await _db.Titles
                .Where(t => t.Id == titleId)
                .Select(t => t.Name)
                .FirstOrDefaultAsync();

            TempData["SavedMsg"] =
                progress.Status == "Completed"
                    ? $"Marked \"{titleName}\" as completed."
                    : $"Progress updated: \"{titleName}\" — Season {season}, Episode {epValue}.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWatched(int titleId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // защита: само за заглавие от твоя каталог
            var ownsTitle = await _db.UserTitles.AnyAsync(ut => ut.UserId == userId && ut.TitleId == titleId);
            if (!ownsTitle) return Forbid();

            var progress = await _db.UserTitleProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId);

            if (progress == null)
            {
                progress = new UserTitleProgress
                {
                    UserId = userId,
                    TitleId = titleId
                };
                _db.UserTitleProgresses.Add(progress);
            }

            progress.Status = "Completed";
            await _db.SaveChangesAsync();

            var titleName = await _db.Titles
                .Where(t => t.Id == titleId)
                .Select(t => t.Name)
                .FirstOrDefaultAsync();

            TempData["SavedMsg"] = $"Marked \"{titleName}\" as completed.";
            return RedirectToAction(nameof(Index));
        }

        // ================= HELPERS =================

        private static int? ParseYear(string? date)
        {
            if (string.IsNullOrWhiteSpace(date) || date.Length < 4)
                return null;

            return int.TryParse(date.Substring(0, 4), out var y) ? y : null;
        }

        private async Task AddGenresAsync(Title title, List<TmdbGenre> tmdbGenres)
        {
            foreach (var g in tmdbGenres)
            {
                var existingGenre = await _db.Genres.FirstOrDefaultAsync(x => x.TmdbId == g.Id);

                if (existingGenre == null)
                {
                    existingGenre = new Genre
                    {
                        TmdbId = g.Id,
                        Name = g.Name
                    };
                    _db.Genres.Add(existingGenre);
                }

                title.TitleGenres.Add(new TitleGenre
                {
                    Genre = existingGenre
                });
            }
        }
    }
}