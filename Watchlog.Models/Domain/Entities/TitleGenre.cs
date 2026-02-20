namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between titles and genres.
    /// </summary>
    public class TitleGenre
    {
        /// <summary>
        /// Gets or sets the foreign key of the associated title.
        /// </summary>
        public int TitleId { get; set; }

        /// <summary>
        /// Navigation property to the related title.
        /// </summary>
        public Title Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key of the associated genre.
        /// </summary>
        public int GenreId { get; set; }

        /// <summary>
        /// Navigation property to the related genre.
        /// </summary>
        public Genre Genre { get; set; } = null!;
    }
}