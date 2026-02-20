namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a classification assigned to titles.
    /// This entity is synchronized with The Movie Database (TMDb)
    /// and linked to titles through a many-to-many relationship.
    /// </summary>
    public class Genre
    {
        /// <summary>
        /// Gets or sets the unique identifier of the genre.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the TMDb identifier used for external mapping.
        /// </summary>
        public int TmdbId { get; set; }

        /// <summary>
        /// Gets or sets the display name of the genre.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Navigation property representing the relationship
        /// between titles and this genre.
        /// </summary>
        public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
    }
}