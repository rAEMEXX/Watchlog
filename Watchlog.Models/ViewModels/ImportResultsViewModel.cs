using Watchlog.Models.Tmdb;

namespace Watchlog.Models.ViewModels
{
    /// <summary>
    /// Represents the results returned from a TMDB import search.
    /// </summary>
    public class ImportResultsViewModel
    {
        /// <summary>
        /// Gets or sets the search query entered by the user.
        /// </summary>
        public string Query { get; set; } = null!;

        /// <summary>
        /// Gets or sets the TV series results returned from TMDB.
        /// </summary>
        public List<TmdbSearchItem> TvResults { get; set; } = new();

        /// <summary>
        /// Gets or sets the movie results returned from TMDB.
        /// </summary>
        public List<TmdbSearchItem> MovieResults { get; set; } = new();
    }
}