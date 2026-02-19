using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserCatalogService(
            IRepository<UserTitle> userTitles,
            IRepository<UserTitleProgress> progress,
            IMapper mapper)
        {
            _userTitles = userTitles;
            _progress = progress;
            _mapper = mapper;
        }

        public async Task<List<TitleCatalogItemViewModel>> GetCatalogAsync(string userId, CancellationToken ct = default)
        {
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

            var result = new List<TitleCatalogItemViewModel>(titles.Count);

            foreach (var t in titles)
            {
                var vm = _mapper.Map<TitleCatalogItemViewModel>(t);

                // Ensure dropdown seasons are sorted (AutoMapper preserves order as-is)
                vm.Seasons = vm.Seasons.OrderBy(s => s.SeasonNumber).ToList();

                // Fill user-specific fields
                if (progressByTitleId.TryGetValue(t.Id, out var p))
                {
                    vm.CurrentSeason = p.CurrentSeason;
                    vm.CurrentEpisode = p.CurrentEpisode;
                    vm.Status = p.Status;
                }

                result.Add(vm);
            }

            return result;
        }

        public async Task RemoveFromCatalogAsync(string userId, int titleId, CancellationToken ct = default)
        {
            // same as your Step 4 implementation
            var link = await _userTitles.Query()
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TitleId == titleId, ct);

            if (link == null) return;

            _userTitles.Delete(link);
            await _userTitles.SaveChangesAsync(ct);
        }
    }
}