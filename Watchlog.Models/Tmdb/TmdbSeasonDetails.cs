using System.Text.Json.Serialization;

namespace Watchlog.Models.Tmdb
{
	public class TmdbSeasonDetails
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("season_number")]
		public int SeasonNumber { get; set; }

		[JsonPropertyName("episodes")]
		public List<TmdbEpisode> Episodes { get; set; } = new();
	}

	public class TmdbEpisode
	{
		[JsonPropertyName("episode_number")]
		public int EpisodeNumber { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("runtime")]
		public int? Runtime { get; set; }
	}
}