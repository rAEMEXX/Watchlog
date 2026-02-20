namespace Watchlog.Models.ViewModels
{
    /// <summary>
    /// Represents a title item displayed in the user's catalog.
    /// </summary>
    public class TitleCatalogItemViewModel
    {
        /// <summary>
        /// Gets or sets the title identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the release year of the title.
        /// </summary>
        public int? ReleaseYear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the title is a TV series.
        /// </summary>
        public bool IsSeries { get; set; }

        /// <summary>
        /// Gets or sets the available seasons for selection.
        /// Used for season and episode dropdowns.
        /// </summary>
        public List<SeasonOption> Seasons { get; set; } = new();

        /// <summary>
        /// Gets or sets the user's current season progress.
        /// </summary>
        public int? CurrentSeason { get; set; }

        /// <summary>
        /// Gets or sets the user's current episode progress.
        /// </summary>
        public int? CurrentEpisode { get; set; }

        /// <summary>
        /// Gets or sets the viewing status of the title.
        /// </summary>
        public string? Status { get; set; }
    }

    /// <summary>
    /// Represents a season option used for progress selection.
    /// </summary>
    public class SeasonOption
    {
        /// <summary>
        /// Gets or sets the season number.
        /// </summary>
        public int SeasonNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of episodes in the season.
        /// </summary>
        public int EpisodeCount { get; set; }
    }
}