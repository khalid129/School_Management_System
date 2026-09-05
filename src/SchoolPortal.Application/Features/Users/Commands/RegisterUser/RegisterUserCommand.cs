using MediatR;

namespace SchoolPortal.Application.Features.Users.Commands.RegisterUser;

/// <summary>
/// Admin-driven registration: creates a global USERS row plus a USER_SCHOOL_MEMBERSHIPS row
/// (Status = Invited) for the current tenant, and returns an invite token. School scope comes
/// from the resolved tenant, not the request.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string FullName,
    string Password,
    string RoleName,
    string? PhoneNumber = null,
    string? PreferredLanguage = null,
    bool IsPrimary = true) : IRequest<RegisterUserResult>;
