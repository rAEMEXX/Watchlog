namespace Watchlog.Models.Domain.Entities
{
	public class Title
	{
		public int Id { get; set; }
		public int TmdbId { get; set; }

		public string Name { get; set; } = null!;
		public bool IsSeries { get; set; }
		public int? ReleaseYear { get; set; }
		public string? Overview { get; set; }

		public ICollection<TitleGenre> TitleGenres { get; set; } = new List<TitleGenre>();
		public ICollection<Season> Seasons { get; set; } = new List<Season>();
	}
}