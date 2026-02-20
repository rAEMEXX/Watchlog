namespace Watchlog.Models.ViewModels
{
    /// <summary>
    /// Represents an admin view model for displaying a user's catalog item.
    /// </summary>
    public class AdminUserCatalogItemViewModel
    {
        /// <summary>
        /// Gets or sets the title identifier.
        /// </summary>
        public int TitleId { get; set; }

        /// <summary>
        /// Gets or sets the title name.
        /// </summary>
        public string TitleName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the total number of seasons for the title.
        /// </summary>
        public int SeasonsCount { get; set; }

        /// <summary>
        /// Gets or sets a dictionary mapping season numbers to episode counts.
        /// Used for season and episode selection dropdowns.
        /// </summary>
        public Dictionary<int, int> EpisodesPerSeason { get; set; } = new();

        /// <summary>
        /// Gets or sets the current season selected by the user.
        /// </summary>
        public int? CurrentSeason { get; set; }

        /// <summary>
        /// Gets or sets the current episode selected by the user.
        /// </summary>
        public int? CurrentEpisode { get; set; }

        /// <summary>
        /// Gets or sets the viewing status (e.g. Watching, Completed, Plan to Watch).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the title was added to the user's catalog.
        /// </summary>
        public DateTime AddedAt { get; set; }
    }
}