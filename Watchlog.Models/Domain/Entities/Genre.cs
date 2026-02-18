namespace WatchLog.Models.Domain.Entities
{
	public class Genre
	{
		public int Id { get; set; }
		public int TmdbId { get; set; }

		public string Name { get; set; } = null!;
		public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
	}
}