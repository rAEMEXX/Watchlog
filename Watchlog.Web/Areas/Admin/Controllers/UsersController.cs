using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watchlog.Data.Persistance;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.ViewModels;

namespace Watchlog.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _db;

    public UsersController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    // GET: /Admin/Users
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync(ct);

        var catalogCounts = await _db.UserTitles
            .GroupBy(x => x.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count, ct);

        var vm = new List<AdminUserListItemViewModel>(users.Count);

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);

            vm.Add(new AdminUserListItemViewModel
            {
                Id = u.Id,
                Email = u.Email ?? u.UserName ?? "(no email)",
                Roles = roles.ToList(),
                CatalogCount = catalogCounts.TryGetValue(u.Id, out var c) ? c : 0
            });
        }

        return View(vm);
    }

    // GET: /Admin/Users/Catalog/{id}
    [HttpGet]
    public async Task<IActionResult> Catalog(string id, string? message, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var titles = await _db.UserTitles
            .Where(ut => ut.UserId == id)
            .Include(ut => ut.Title)
                .ThenInclude(t => t.Seasons)
            .OrderByDescending(ut => ut.AddedAt)
            .ToListAsync(ct);

        var progressByTitleId = await _db.UserTitleProgresses
            .Where(p => p.UserId == id)
            .ToDictionaryAsync(p => p.TitleId, ct);

        var vm = new AdminUserCatalogViewModel
        {
            UserId = user.Id,
            UserEmail = user.Email ?? user.UserName ?? "(no email)",
            Message = message
        };

        foreach (var ut in titles)
        {
            progressByTitleId.TryGetValue(ut.TitleId, out var p);

            // Build dictionary seasonNumber -> episodeCount
            var eps = (ut.Title.Seasons ?? new List<Season>())
                .Where(s => s.SeasonNumber > 0)
                .OrderBy(s => s.SeasonNumber)
                .ToDictionary(s => s.SeasonNumber, s => s.EpisodeCount);

            vm.Items.Add(new AdminUserCatalogItemViewModel
            {
                TitleId = ut.TitleId,
                TitleName = ut.Title.Name,
                SeasonsCount = ut.Title.Seasons?.Count ?? 0,
                EpisodesPerSeason = eps,

                CurrentSeason = p?.CurrentSeason,
                CurrentEpisode = p?.CurrentEpisode,
                Status = p?.Status ?? "",
                AddedAt = ut.AddedAt
            });
        }

        return View(vm);
    }

    // POST: /Admin/Users/RemoveTitle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTitle(string userId, int titleId, CancellationToken ct)
    {
        var link = await _db.UserTitles
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TitleId == titleId, ct);

        if (link != null)
            _db.UserTitles.Remove(link);

        var progress = await _db.UserTitleProgresses
            .FirstOrDefaultAsync(x => x.UserId == userId && x.TitleId == titleId, ct);

        if (progress != null)
            _db.UserTitleProgresses.Remove(progress);

        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Catalog), new { id = userId, message = "Removed title from catalog." });
    }

    // POST: /Admin/Users/UpdateProgress
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProgress(string userId, int titleId, int season, int? episode, CancellationToken ct)
    {
        // ✅ Do not create progress for movies / titles with 0 seasons
        var seasonsCount = await _db.Seasons.CountAsync(s => s.TitleId == titleId, ct);
        if (seasonsCount <= 0)
        {
            return RedirectToAction(nameof(Catalog), new { id = userId, message = "This title has no seasons. Progress can't be edited." });
        }

        if (season < 1) season = 1;

        // If episode was left blank in the form, treat it as 1
        var epValue = episode ?? 1;
        if (epValue < 1) epValue = 1;

        // Clamp episode to season max (if season exists)
        var seasonRow = await _db.Seasons
            .Where(s => s.TitleId == titleId && s.SeasonNumber == season)
            .Select(s => new { s.EpisodeCount })
            .FirstOrDefaultAsync(ct);

        if (seasonRow != null && seasonRow.EpisodeCount > 0 && epValue > seasonRow.EpisodeCount)
            epValue = seasonRow.EpisodeCount;

        var progress = await _db.UserTitleProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.TitleId == titleId, ct);

        if (progress == null)
        {
            progress = new UserTitleProgress
            {
                UserId = userId,
                TitleId = titleId
            };
            await _db.UserTitleProgresses.AddAsync(progress, ct);
        }

        progress.CurrentSeason = season;
        progress.CurrentEpisode = epValue;

        // Completed if last season + last episode
        var lastSeason = await _db.Seasons
            .Where(s => s.TitleId == titleId)
            .OrderByDescending(s => s.SeasonNumber)
            .Select(s => new { s.SeasonNumber, s.EpisodeCount })
            .FirstOrDefaultAsync(ct);

        if (lastSeason != null && season == lastSeason.SeasonNumber && epValue == lastSeason.EpisodeCount)
            progress.Status = "Completed";
        else
            progress.Status = "Watching";

        await _db.SaveChangesAsync(ct);

        return RedirectToAction(nameof(Catalog), new { id = userId, message = "Progress updated." });
    }
}