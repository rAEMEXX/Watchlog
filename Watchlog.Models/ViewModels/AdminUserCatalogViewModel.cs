namespace Watchlog.Models.ViewModels
{
    /// <summary>
    /// Represents an admin view model for displaying a user's catalog.
    /// </summary>
    public class AdminUserCatalogViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string UserEmail { get; set; } = null!;

        /// <summary>
        /// Gets or sets the catalog items belonging to the user.
        /// </summary>
        public List<AdminUserCatalogItemViewModel> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets an optional informational message to display in the UI.
        /// </summary>
        public string? Message { get; set; }
    }
}