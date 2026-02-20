namespace Watchlog.Models.ViewModels;

public class AdminUserListItemViewModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public int CatalogCount { get; set; }
}