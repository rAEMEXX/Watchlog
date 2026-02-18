using WatchLog.Models.Tmdb;

namespace WatchLog.Models.ViewModels
{
	public class ImportResultsViewModel
	{
		public string Query { get; set; } = null!;

		public List<TmdbSearchItem> TvResults { get; set; } = new();
		public List<TmdbSearchItem> MovieResults { get; set; } = new();
	}
}