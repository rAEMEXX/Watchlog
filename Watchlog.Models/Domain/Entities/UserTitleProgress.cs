using Microsoft.AspNetCore.Identity;

namespace Watchlog.Models.Domain.Entities
{
    public class UserTitleProgress
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        public int TitleId { get; set; }
        public Title Title { get; set; } = null!;

        public int? CurrentSeason { get; set; }
        public int? CurrentEpisode { get; set; }

        public string Status { get; set; } = "Watching";
    }
}