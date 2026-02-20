namespace Watchlog.Models.ViewModels;

public class AdminUserCatalogItemViewModel
{
    public int TitleId { get; set; }
    public string TitleName { get; set; } = null!;

    // Total seasons
    public int SeasonsCount { get; set; }

    // For dropdowns: seasonNumber -> episodeCount
    public Dictionary<int, int> EpisodesPerSeason { get; set; } = new();

    public int? CurrentSeason { get; set; }
    public int? CurrentEpisode { get; set; }
    public string Status { get; set; } = "";

    public DateTime AddedAt { get; set; }
}