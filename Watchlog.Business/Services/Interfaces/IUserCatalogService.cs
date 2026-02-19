using Watchlog.Models.ViewModels;

namespace Watchlog.Business.Services.Interfaces
{
    public interface IUserCatalogService
    {
        Task<List<TitleCatalogItemViewModel>> GetCatalogAsync(string userId, CancellationToken ct = default);
        Task RemoveFromCatalogAsync(string userId, int titleId, CancellationToken ct = default);
    }
}