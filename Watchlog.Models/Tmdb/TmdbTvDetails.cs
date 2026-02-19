using System.Text.Json.Serialization;

namespace Watchlog.Models.Tmdb
{
	public class TmdbTvDetails
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; } = null!;

		[JsonPropertyName("first_air_date")]
		public string? FirstAirDate { get; set; }

		[JsonPropertyName("overview")]
		public string? Overview { get; set; }

		[JsonPropertyName("number_of_seasons")]
		public int NumberOfSeasons { get; set; }

		[JsonPropertyName("seasons")]
		public List<TmdbTvSeasonStub> Seasons { get; set; } = new();

        [JsonPropertyName("genres")]
        public List<TmdbGenre> Genres { get; set; } = new();
    }

    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }

    public class TmdbTvSeasonStub
	{
		[JsonPropertyName("season_number")]
		public int SeasonNumber { get; set; }

		[JsonPropertyName("episode_count")]
		public int EpisodeCount { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }
	}
}