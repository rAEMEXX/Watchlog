using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watchlog.Business.Services.Interfaces;

namespace Watchlog.Controllers
{
    [Authorize]
    public class TitlesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICatalogImportService _import;
        private readonly IUserCatalogService _catalog;
        private readonly IUserProgressService _progress;

        public TitlesController(
            UserManager<IdentityUser> userManager,
            ICatalogImportService import,
            IUserCatalogService catalog,
            IUserProgressService progress)
        {
            _userManager = userManager;
            _import = import;
            _catalog = catalog;
            _progress = progress;
        }

        [HttpGet]
        public IActionResult Import() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(string query, CancellationToken ct)
        {
            try
            {
                var model = await _import.SearchAsync(query, ct);
                return View("ImportResults", model);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSelected(int tmdbId, string type, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var titleName = await _import.ImportSelectedAsync(userId, tmdbId, type, ct);
            TempData["SavedMsg"] = $"Added \"{titleName}\" to your catalog.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var model = await _catalog.GetCatalogAsync(userId, ct);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCatalog(int titleId, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            await _catalog.RemoveFromCatalogAsync(userId, titleId, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int titleId, int season, int? episode, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            try
            {
                TempData["SavedMsg"] = await _progress.UpdateProgressAsync(userId, titleId, season, episode, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWatched(int titleId, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            try
            {
                TempData["SavedMsg"] = await _progress.MarkWatchedAsync(userId, titleId, ct);
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}