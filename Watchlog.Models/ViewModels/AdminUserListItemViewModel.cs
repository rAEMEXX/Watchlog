namespace Watchlog.Models.ViewModels
{
    /// <summary>
    /// Represents an admin view model for displaying a user in the user list.
    /// </summary>
    public class AdminUserListItemViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the roles assigned to the user.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Gets or sets the number of titles in the user's catalog.
        /// </summary>
        public int CatalogCount { get; set; }
    }
}