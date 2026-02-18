using System.Text.Json.Serialization;

namespace WatchLog.Models.Tmdb
{
	public class TmdbSearchResult
	{
		[JsonPropertyName("results")]
		public List<TmdbSearchItem> Results { get; set; } = new();
	}
}
