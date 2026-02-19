namespace Watchlog.Models.Domain.Entities
{
	public class Season
	{
		public int Id { get; set; }

		public int TitleId { get; set; }
		public Title Title { get; set; } = null!;

		public int SeasonNumber { get; set; }
		public int EpisodeCount { get; set; }

		public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
	}
}