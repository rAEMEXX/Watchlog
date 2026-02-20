namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a single episode within a television season.
    /// </summary>
    public class Episode
    {
        /// <summary>
        /// Gets or sets the unique identifier of the episode.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the season this episode belongs to.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// Navigation property to the season.
        /// </summary>
        public Season Season { get; set; } = null!;

        /// <summary>
        /// Gets or sets the episode number within the season.
        /// </summary>
        public int EpisodeNumber { get; set; }

        /// <summary>
        /// Gets or sets the episode title.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the runtime of the episode in minutes.
        /// </summary>
        public int? RuntimeMinutes { get; set; }
    }
}