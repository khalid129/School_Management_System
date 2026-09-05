using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Infrastructure.Identity;

/// <summary>
/// Reads the caller's USERS.ID from the token. Until auth ships this is normally null;
/// the audit columns then record null for CreatedBy/UpdatedBy, which is acceptable.
/// </summary>
public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var principal = httpContextAccessor.HttpContext?.User;
            var raw =
                principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                principal?.FindFirstValue("sub");

            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
}
