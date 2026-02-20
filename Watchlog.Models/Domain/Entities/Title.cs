namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a movie or television series imported from 
    /// The Movie Database (TMDb).
    /// </summary>
    public class Title
    {
        /// <summary>
        /// Gets or sets the unique identifier of the title.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the TMDb identifier used for external synchronization.
        /// </summary>
        public int TmdbId { get; set; }

        /// <summary>
        /// Gets or sets the name of the movie or series.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets a value indicating whether the title is a television series.
        /// True = series, False = movie.
        /// </summary>
        public bool IsSeries { get; set; }

        /// <summary>
        /// Gets or sets the release year of the title, if known.
        /// </summary>
        public int? ReleaseYear { get; set; }

        /// <summary>
        /// Gets or sets a brief description or overview of the title.
        /// </summary>
        public string? Overview { get; set; }

        /// <summary>
        /// Navigation property representing the genres assigned to this title.
        /// </summary>
        public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();

        /// <summary>
        /// Navigation property representing the seasons associated with this title.
        /// Applicable only when the title is a television series.
        /// </summary>
        public ICollection<Season> Seasons { get; set; } = new List<Season>();
    }
}