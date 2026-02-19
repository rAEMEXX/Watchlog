using AutoMapper;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Business.Services.Interfaces;
using Watchlog.Models.Domain.Entities;
// other usings...

public class CatalogImportService : ICatalogImportService
{
    private readonly ITmdbService _tmdb;
    private readonly IRepository<Title> _titles;
    private readonly IRepository<Genre> _genres;
    private readonly IRepository<UserTitle> _userTitles;
    private readonly IMapper _mapper;

    public CatalogImportService(
        ITmdbService tmdb,
        IRepository<Title> titles,
        IRepository<Genre> genres,
        IRepository<UserTitle> userTitles,
        IMapper mapper)
    {
        _tmdb = tmdb;
        _titles = titles;
        _genres = genres;
        _userTitles = userTitles;
        _mapper = mapper;
    }

    // inside ImportSelectedAsync:
    // TV:
    var tv = await _tmdb.GetTvDetailsAsync(tmdbId) ?? throw new InvalidOperationException("TMDB TV title not found.");
    var title = _mapper.Map<Title>(tv);

    // map seasons (skip specials season 0)
    title.Seasons = tv.Seasons
        .Where(s => s.SeasonNumber > 0)
        .Select(s => _mapper.Map<Season>(s))
        .ToList();

    // genres stay manual (find or create in DB by TmdbId)
    await AddGenresAsync(title, tv.Genres, ct);

    await _titles.AddAsync(title, ct);
    await _titles.SaveChangesAsync(ct);
}