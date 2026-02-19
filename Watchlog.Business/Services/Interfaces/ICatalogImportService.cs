using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Interfaces
{
    public interface ICatalogImportService
    {
        Task<ImportResultsViewModel> SearchAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Imports a TMDB title (if missing) and links it to the user's catalog (UserTitles).
        /// Returns the saved Title name for UI messaging.
        /// </summary>
        Task<string> ImportSelectedAsync(string userId, int tmdbId, string type, CancellationToken ct = default);
    }
}