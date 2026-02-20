using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Interfaces
{
    /// <summary>
    /// Defines a service for managing a user's personal title catalog.
    /// Provides functionality for retrieving and modifying the user's saved titles.
    /// </summary>
    public interface IUserCatalogService
    {
        /// <summary>
        /// Retrieves the catalog of titles saved by the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        /// <returns>
        /// A list of catalog items formatted for display in the user interface.
        /// </returns>
        Task<List<TitleCatalogItemViewModel>> GetCatalogAsync(string userId, CancellationToken ct = default);

        /// <summary>
        /// Removes a title from the user's catalog.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="titleId">The identifier of the title to remove.</param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        Task RemoveFromCatalogAsync(string userId, int titleId, CancellationToken ct = default);
    }
}