using Microsoft.AspNetCore.Identity;

namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// Represents a title saved or tracked by a specific user.
    /// </summary>
    public class UserTitle
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user-title entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the user who added the title.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the associated user.
        /// </summary>
        public IdentityUser User { get; set; } = null!;

        /// <summary>
        /// Gets or sets the foreign key of the tracked title.
        /// </summary>
        public int TitleId { get; set; }

        /// <summary>
        /// Navigation property to the associated title.
        /// </summary>
        public Title Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the date and time when the title was added by the user.
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}