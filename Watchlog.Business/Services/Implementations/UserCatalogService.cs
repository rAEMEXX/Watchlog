using Microsoft.EntityFrameworkCore;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Implementations
{
    public class UserCatalogService : IUserCatalogService
    {
        private readonly IRepository<UserTitle> _userTitles;
        private readonly IRepository<UserTitleProgress> _progress;
        private readonly IRepository<Title> _titles;

        public UserCatalogService(
            IRepository<UserTitle> userTitles,
            IRepository<UserTitleProgress> progress,
            IRepository<Title> titles)
        {
            _userTitles = userTitles;
            _progress = progress;
            _titles = titles;
        }

        public async Task<List<TitleCatalogItemViewModel>> GetCatalogAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            var titles = await _userTitles.Query()
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.Title)
                    .ThenInclude(t => t.Seasons)
                .OrderByDescending(ut => ut.AddedAt)
                .Select(ut => ut.Title)
                .ToListAsync(ct);

            var progressByTitleId = await _progress.Query()
                .Where(p => p.UserId == userId)
                .ToDictionaryAsync(p => p.TitleId, ct);

            return titles.Select(t =>
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
        }

        public async Task RemoveFromCatalogAsync(string userId, int titleId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("User not logged in.");

            var link = await _userTitles.Query()
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TitleId == titleId, ct);

            if (link != null)
            {
                _userTitles.Delete(link);

                var prog = await _progress.Query()
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId, ct);

                if (prog != null)
                    _progress.Delete(prog);

                await _userTitles.SaveChangesAsync(ct);
                await _progress.SaveChangesAsync(ct);
            }
        }
    }
}