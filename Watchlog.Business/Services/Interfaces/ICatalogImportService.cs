using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Interfaces
{
    /// <summary>
    /// Defines a service responsible for searching and importing titles
    /// from an external catalog (e.g. TMDb) into the user's personal collection.
    /// </summary>
    public interface ICatalogImportService
    {
        /// <summary>
        /// Searches the external catalog for titles matching the specified query.
        /// </summary>
        /// <param name="query">The search keyword or phrase entered by the user.</param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        /// <returns>
        /// A view model containing the search results ready for display.
        /// </returns>
        Task<ImportResultsViewModel> SearchAsync(string query, CancellationToken ct = default);

        /// <summary>
        /// Imports a selected title into the user's catalog.
        /// </summary>
        /// <param name="userId">The unique identifier of the user importing the title.</param>
        /// <param name="tmdbId">The external TMDb identifier of the selected title.</param>
        /// <param name="type">The type of title (e.g. "movie" or "tv").</param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        /// <returns>
        /// A status message describing the result of the import operation.
        /// </returns>
        Task<string> ImportSelectedAsync(string userId, int tmdbId, string type, CancellationToken ct = default);
    }
}