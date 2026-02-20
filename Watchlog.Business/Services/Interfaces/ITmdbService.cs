using Watchlog.Models.Tmdb;

namespace Watchlog.Business.Services.Interfaces
{
    /// <summary>
    /// Defines a service for communicating with the TMDb API
    /// to retrieve movie and TV show data.
    /// </summary>
    public interface ITmdbService
    {
        /// <summary>
        /// Searches TMDb for TV series matching the specified query.
        /// </summary>
        /// <param name="query">The search keyword or phrase.</param>
        /// <returns>
        /// A search result containing matching TV series, or null if the request fails.
        /// </returns>
        Task<TmdbSearchResult?> SearchTvAsync(string query);

        /// <summary>
        /// Searches TMDb for movies matching the specified query.
        /// </summary>
        /// <param name="query">The search keyword or phrase.</param>
        /// <returns>
        /// A search result containing matching movies, or null if the request fails.
        /// </returns>
        Task<TmdbSearchResult?> SearchMovieAsync(string query);

        /// <summary>
        /// Retrieves detailed information about a specific TV series.
        /// </summary>
        /// <param name="tvId">The TMDb identifier of the TV series.</param>
        /// <returns>
        /// Detailed TV series information, or null if not found.
        /// </returns>
        Task<TmdbTvDetails?> GetTvDetailsAsync(int tvId);

        /// <summary>
        /// Retrieves detailed information for a specific season of a TV series.
        /// </summary>
        /// <param name="tvId">The TMDb identifier of the TV series.</param>
        /// <param name="seasonNumber">The season number to retrieve.</param>
        /// <returns>
        /// Season details including episodes, or null if not found.
        /// </returns>
        Task<TmdbSeasonDetails?> GetSeasonDetailsAsync(int tvId, int seasonNumber);

        /// <summary>
        /// Retrieves detailed information about a specific movie.
        /// </summary>
        /// <param name="movieId">The TMDb identifier of the movie.</param>
        /// <returns>
        /// Detailed movie information, or null if not found.
        /// </returns>
        Task<TmdbMovieDetails?> GetMovieDetailsAsync(int movieId);
    }
}