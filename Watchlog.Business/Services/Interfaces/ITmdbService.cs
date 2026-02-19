using Watchlog.Models.Tmdb;

namespace Watchlog.Business.Services.Interfaces
{
    public interface ITmdbService
    {
        Task<TmdbSearchResult?> SearchTvAsync(string query);
        Task<TmdbSearchResult?> SearchMovieAsync(string query);

        Task<TmdbTvDetails?> GetTvDetailsAsync(int tvId);
        Task<TmdbSeasonDetails?> GetSeasonDetailsAsync(int tvId, int seasonNumber);

        Task<TmdbMovieDetails?> GetMovieDetailsAsync(int movieId);
    }
}
