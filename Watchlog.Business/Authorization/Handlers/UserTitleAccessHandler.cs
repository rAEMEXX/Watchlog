using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Watchlog.Business.Authorization.Requirements;
using Watchlog.Business.Repositories.Interfaces;
using Watchlog.Models.Domain.Entities;

namespace Watchlog.Business.Authorization.Handlers;

public class UserTitleAccessHandler
    : AuthorizationHandler<UserTitleAccessRequirement, int> // resource = titleId
{
    private readonly IRepository<UserTitle> _userTitles;

    public UserTitleAccessHandler(IRepository<UserTitle> userTitles)
    {
        _userTitles = userTitles;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        UserTitleAccessRequirement requirement,
        int titleId)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return;

        var roles = context.User.FindAll(ClaimTypes.Role).Select(r => r.Value);

        // Admin can do anything
        if (roles.Contains("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Normal users: must own the title in their catalog
        var hasTitle = await _userTitles.Query()
            .AnyAsync(ut => ut.UserId == userId && ut.TitleId == titleId);

        if (hasTitle)
            context.Succeed(requirement);
    }
}