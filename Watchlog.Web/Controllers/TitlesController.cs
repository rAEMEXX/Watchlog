using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Watchlog.Business.Authorization;              // Policies
using Watchlog.Business.Services.Interfaces;

namespace Watchlog.Controllers
{
    [Authorize]
    public class TitlesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorization;
        private readonly ICatalogImportService _import;
        private readonly IUserCatalogService _catalog;
        private readonly IUserProgressService _progress;

        public TitlesController(
            UserManager<IdentityUser> userManager,
            IAuthorizationService authorization,
            ICatalogImportService import,
            IUserCatalogService catalog,
            IUserProgressService progress)
        {
            _userManager = userManager;
            _authorization = authorization;
            _import = import;
            _catalog = catalog;
            _progress = progress;
        }

        // ---------- IMPORT ----------

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

        // ---------- CATALOG ----------

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

            // ✅ Foodie-style: authorize at controller boundary
            var auth = await _authorization.AuthorizeAsync(User, titleId, Policies.UserTitleAccessPolicy);
            if (!auth.Succeeded)
                return Forbid();

            await _catalog.RemoveFromCatalogAsync(userId, titleId, ct);
            TempData["SavedMsg"] = "Removed from your catalog.";
            return RedirectToAction(nameof(Index));
        }

        // ---------- PROGRESS ----------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProgress(int titleId, int season, int? episode, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // ✅ Policy check first (no try/catch needed)
            var auth = await _authorization.AuthorizeAsync(User, titleId, Policies.UserTitleAccessPolicy);
            if (!auth.Succeeded)
                return Forbid();

            TempData["SavedMsg"] = await _progress.UpdateProgressAsync(userId, titleId, season, episode, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWatched(int titleId, CancellationToken ct)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // ✅ Policy check first
            var auth = await _authorization.AuthorizeAsync(User, titleId, Policies.UserTitleAccessPolicy);
            if (!auth.Succeeded)
                return Forbid();

            TempData["SavedMsg"] = await _progress.MarkWatchedAsync(userId, titleId, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}