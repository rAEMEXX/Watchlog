using AutoMapper;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.Tmdb;

namespace Watchlog.Business.Mappings
{
    /// <summary>
    /// Defines mappings between TMDb API models and domain entities
    /// used for importing movies, TV series, and season data.
    /// </summary>
    public class TmdbMappingProfile : Profile
    {
        public TmdbMappingProfile()
        {
            CreateMap<TmdbTvDetails, Title>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TmdbId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsSeries, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => ParseYear(src.FirstAirDate)))
                .ForMember(dest => dest.Overview, opt => opt.MapFrom(src => src.Overview))
                .ForMember(dest => dest.Seasons, opt => opt.Ignore())
                .ForMember(dest => dest.TitleGenres, opt => opt.Ignore());

            CreateMap<TmdbMovieDetails, Title>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TmdbId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsSeries, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ReleaseYear, opt => opt.MapFrom(src => ParseYear(src.ReleaseDate)))
                .ForMember(dest => dest.Overview, opt => opt.MapFrom(src => src.Overview))
                .ForMember(dest => dest.Seasons, opt => opt.Ignore())
                .ForMember(dest => dest.TitleGenres, opt => opt.Ignore());

            // Stub -> Season entity (TitleId/Title are set later)
            CreateMap<TmdbTvSeasonStub, Season>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TitleId, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore())
                .ForMember(dest => dest.SeasonNumber, opt => opt.MapFrom(src => src.SeasonNumber))
                .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.EpisodeCount));
        }

        private static int? ParseYear(string? date)
        {
            if (string.IsNullOrWhiteSpace(date) || date.Length < 4)
                return null;

            return int.TryParse(date.Substring(0, 4), out var y) ? y : null;
        }
    }
}