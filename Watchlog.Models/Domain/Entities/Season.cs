namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a season of a television series, containing multiple episodes.
    /// </summary>
    public class Season
    {
        /// <summary>
        /// Gets or sets the unique identifier of the season.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the title (TV series) this season belongs to.
        /// </summary>
        public int TitleId { get; set; }

        /// <summary>
        /// Navigation property to the parent title.
        /// </summary>
        public Title Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the season number within the series.
        /// </summary>
        public int SeasonNumber { get; set; }

        /// <summary>
        /// Gets or sets the total number of episodes in the season.
        /// </summary>
        public int EpisodeCount { get; set; }

        /// <summary>
        /// Navigation property representing the episodes included in this season.
        /// </summary>
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}