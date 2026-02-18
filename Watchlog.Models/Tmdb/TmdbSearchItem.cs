using System.Text.Json.Serialization;

namespace WatchLog.Models.Tmdb
{
	public class TmdbSearchItem
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		// При TV search
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("first_air_date")]
		public string? FirstAirDate { get; set; }

		// При Movie search
		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("release_date")]
		public string? ReleaseDate { get; set; }

		[JsonPropertyName("overview")]
		public string? Overview { get; set; }

		[JsonIgnore]
		public string DisplayName => Name ?? Title ?? "(no title)";

		[JsonIgnore]
		public string? DisplayDate => FirstAirDate ?? ReleaseDate;
	}
}