namespace Watchlog.Models.ViewModels;

public class AdminUserCatalogViewModel
{
    public string UserId { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public List<AdminUserCatalogItemViewModel> Items { get; set; } = new();

    public string? Message { get; set; }
}