using Microsoft.AspNetCore.Identity;

namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a user's viewing progress and status for a specific title.
    /// </summary>
    public class UserTitleProgress
    {
        /// <summary>
        /// Gets or sets the unique identifier of the progress entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the user whose progress is tracked.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the associated user.
        /// </summary>
        public IdentityUser User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key of the title being tracked.
        /// </summary>
        public int TitleId { get; set; }

        /// <summary>
        /// Navigation property to the associated title.
        /// </summary>
        public Title Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current season the user has reached.
        /// </summary>
        public int? CurrentSeason { get; set; }

        /// <summary>
        /// Gets or sets the current episode the user has reached.
        /// </summary>
        public int? CurrentEpisode { get; set; }

        /// <summary>
        /// Gets or sets the viewing status of the title (e.g., Watching, Completed, Planned).
        /// </summary>
        public string Status { get; set; } = "Watching";
    }
}