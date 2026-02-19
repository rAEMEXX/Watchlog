using Microsoft.AspNetCore.Identity;

namespace Watchlog.Models.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class UserTitle
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        public int TitleId { get; set; }
        public Title Title { get; set; } = null!;

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}