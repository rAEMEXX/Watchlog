namespace Watchlog.Business.Services.Interfaces
{
    public interface IUserProgressService
    {
        /// <summary>
        /// Updates (or creates) progress row for the user+title.
        /// Returns a message for TempData.
        /// </summary>
        Task<string> UpdateProgressAsync(string userId, int titleId, int season, int? episode, CancellationToken ct = default);

        Task<string> MarkWatchedAsync(string userId, int titleId, CancellationToken ct = default);
    }
}