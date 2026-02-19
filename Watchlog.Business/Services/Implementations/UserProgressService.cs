using Microsoft.EntityFrameworkCore;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Models.Domain.Entities;

namespace Watchlog.Business.Services.Implementations
{
    public class UserProgressService : IUserProgressService
    {
        private readonly IRepository<UserTitle> _userTitles;
        private readonly IRepository<UserTitleProgress> _progress;
        private readonly IRepository<Season> _seasons;
        private readonly IRepository<Title> _titles;

        public UserProgressService(
            IRepository<UserTitle> userTitles,
            IRepository<UserTitleProgress> progress,
            IRepository<Season> seasons,
            IRepository<Title> titles)
        {
            _userTitles = userTitles;
            _progress = progress;
            _seasons = seasons;
            _titles = titles;
        }

        public async Task<string> UpdateProgressAsync(string userId, int titleId, int season, int? episode, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            // Ownership check: must be in user's catalog
            var ownsTitle = await _userTitles.Query()
                .AsNoTracking()
                .AnyAsync(ut => ut.UserId == userId && ut.TitleId == titleId, ct);

            if (!ownsTitle)
                throw new UnauthorizedAccessException("Not in your catalog.");

            var epValue = episode ?? 1;
            if (season < 1) season = 1;
            if (epValue < 1) epValue = 1;

            // Clamp episode to season max
            var seasonRow = await _seasons.Query()
                .Where(s => s.TitleId == titleId && s.SeasonNumber == season)
                .Select(s => new { s.EpisodeCount })
                .FirstOrDefaultAsync(ct);

            if (seasonRow != null && seasonRow.EpisodeCount > 0 && epValue > seasonRow.EpisodeCount)
                epValue = seasonRow.EpisodeCount;

            var progress = await _progress.Query()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId, ct);

            if (progress == null)
            {
                progress = new UserTitleProgress
                {
                    UserId = userId,
                    TitleId = titleId
                };
                await _progress.AddAsync(progress, ct);
            }

            progress.CurrentSeason = season;
            progress.CurrentEpisode = epValue;

            // Completed if last season + last episode
            var lastSeason = await _seasons.Query()
                .Where(s => s.TitleId == titleId)
                .OrderByDescending(s => s.SeasonNumber)
                .FirstOrDefaultAsync(ct);

            if (lastSeason != null && season == lastSeason.SeasonNumber && epValue == lastSeason.EpisodeCount)
                progress.Status = "Completed";
            else
                progress.Status = "Watching";

            await _progress.SaveChangesAsync(ct);

            var titleName = await _titles.Query()
                .Where(t => t.Id == titleId)
                .Select(t => t.Name)
                .FirstOrDefaultAsync(ct);

            return progress.Status == "Completed"
                ? $"Marked \"{titleName}\" as completed."
                : $"Progress updated: \"{titleName}\" — Season {season}, Episode {epValue}.";
        }

        public async Task<string> MarkWatchedAsync(string userId, int titleId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            var ownsTitle = await _userTitles.Query()
                .AsNoTracking()
                .AnyAsync(ut => ut.UserId == userId && ut.TitleId == titleId, ct);

            if (!ownsTitle)
                throw new UnauthorizedAccessException("Not in your catalog.");

            var progress = await _progress.Query()
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId, ct);

            if (progress == null)
            {
                progress = new UserTitleProgress
                {
                    UserId = userId,
                    TitleId = titleId
                };
                await _progress.AddAsync(progress, ct);
            }

            progress.Status = "Completed";
            await _progress.SaveChangesAsync(ct);

            var titleName = await _titles.Query()
                .Where(t => t.Id == titleId)
                .Select(t => t.Name)
                .FirstOrDefaultAsync(ct);

            return $"Marked \"{titleName}\" as completed.";
        }
    }
}