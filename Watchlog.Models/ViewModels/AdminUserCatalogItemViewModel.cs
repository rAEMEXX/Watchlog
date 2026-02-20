namespace Watchlog.Models.ViewModels;

public class AdminUserCatalogItemViewModel
{
    public int TitleId { get; set; }
    public string TitleName { get; set; } = null!;

    public int SeasonsCount { get; set; }
    public int? CurrentSeason { get; set; }
    public int? CurrentEpisode { get; set; }
    public string Status { get; set; } = "Watching";

    public DateTime AddedAt { get; set; }
}