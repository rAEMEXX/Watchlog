namespace Watchlog.Models.ViewModels
{
    public class TitleCatalogItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? ReleaseYear { get; set; }
        public bool IsSeries { get; set; }

        // за dropdown-и
        public List<SeasonOption> Seasons { get; set; } = new();

        // текущ прогрес (per user)
        public int? CurrentSeason { get; set; }
        public int? CurrentEpisode { get; set; }
        public string? Status { get; set; }
    }

    public class SeasonOption
    {
        public int SeasonNumber { get; set; }
        public int EpisodeCount { get; set; }
    }
}