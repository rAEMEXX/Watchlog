using AutoMapper;
using Watchlog.Models.Domain.Entities;
using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Mappings
{
    /// <summary>
    /// Defines mappings between title-related domain entities and
    /// view models used in the catalog and selection interfaces.
    /// </summary>
    public class TitleMappingProfile : Profile
    {
        public TitleMappingProfile()
        {
            // Domain -> ViewModel
            CreateMap<Title, TitleCatalogItemViewModel>()
                // Seasons list is for dropdowns
                .ForMember(dest => dest.Seasons,
                    opt => opt.MapFrom(src => src.Seasons))
                // These are user-specific fields, set later in service
                .ForMember(dest => dest.CurrentSeason, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentEpisode, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<Season, SeasonOption>();
        }
    }
}