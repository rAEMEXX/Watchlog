namespace Watchlog.Business.Services.Interfaces
{
    /// <summary>
    /// Defines a service responsible for tracking and updating
    /// a user's viewing progress for catalog titles.
    /// </summary>
    public interface IUserProgressService
    {
        /// <summary>
        /// Updates the viewing progress for a specific title.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="titleId">The identifier of the title being updated.</param>
        /// <param name="season">The season number currently watched.</param>
        /// <param name="episode">
        /// The episode number currently watched. 
        /// Null may be used for titles without episodes (e.g. movies).
        /// </param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        /// <returns>
        /// A status message describing the result of the update operation.
        /// </returns>
        Task<string> UpdateProgressAsync(string userId, int titleId, int season, int? episode, CancellationToken ct = default);

        /// <summary>
        /// Marks a title as fully watched for the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="titleId">The identifier of the title to mark as watched.</param>
        /// <param name="ct">Cancellation token used to cancel the operation.</param>
        /// <returns>
        /// A status message describing the result of the operation.
        /// </returns>
        Task<string> MarkWatchedAsync(string userId, int titleId, CancellationToken ct = default);
    }
}