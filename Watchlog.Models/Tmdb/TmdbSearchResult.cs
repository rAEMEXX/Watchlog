using System.Text.Json.Serialization;

namespace Watchlog.Models.Tmdb
{
	public class TmdbSearchResult
	{
		[JsonPropertyName("results")]
		public List<TmdbSearchItem> Results { get; set; } = new();
	}
}
