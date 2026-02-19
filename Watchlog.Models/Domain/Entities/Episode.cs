namespace Watchlog.Models.Domain.Entities
{
	public class Episode
	{
		public int Id { get; set; }

		public int SeasonId { get; set; }
		public Season Season { get; set; } = null!;

		public int EpisodeNumber { get; set; }
		public string? Name { get; set; }
		public int? RuntimeMinutes { get; set; }
	}
}