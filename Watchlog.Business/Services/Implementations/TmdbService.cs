using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Watchlog.Business.Options;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Models.Tmdb;

namespace Watchlog.Business.Services.Implementations
{
    public class TmdbService : ITmdbService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TmdbOptions _options;

        public TmdbService(IHttpClientFactory httpClientFactory, IOptions<TmdbOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<TmdbSearchResult?> SearchTvAsync(string query)
        {
            var client = _httpClientFactory.CreateClient("tmdb");
            var url = $"search/tv?api_key={_options.ApiKey}&query={Uri.EscapeDataString(query)}";
            return await client.GetFromJsonAsync<TmdbSearchResult>(url);
        }

        public async Task<TmdbSearchResult?> SearchMovieAsync(string query)
        {
            var client = _httpClientFactory.CreateClient("tmdb");
            var url = $"search/movie?api_key={_options.ApiKey}&query={Uri.EscapeDataString(query)}";
            return await client.GetFromJsonAsync<TmdbSearchResult>(url);
        }

        public async Task<TmdbTvDetails?> GetTvDetailsAsync(int tvId)
        {
            var client = _httpClientFactory.CreateClient("tmdb");
            var url = $"tv/{tvId}?api_key={_options.ApiKey}";
            return await client.GetFromJsonAsync<TmdbTvDetails>(url);
        }

        public async Task<TmdbSeasonDetails?> GetSeasonDetailsAsync(int tvId, int seasonNumber)
        {
            var client = _httpClientFactory.CreateClient("tmdb");
            var url = $"tv/{tvId}/season/{seasonNumber}?api_key={_options.ApiKey}";
            return await client.GetFromJsonAsync<TmdbSeasonDetails>(url);
        }
        public async Task<TmdbMovieDetails?> GetMovieDetailsAsync(int movieId)
        {
            var client = _httpClientFactory.CreateClient("tmdb");
            var url = $"movie/{movieId}?api_key={_options.ApiKey}";
            return await client.GetFromJsonAsync<TmdbMovieDetails>(url);
        }
    }
}