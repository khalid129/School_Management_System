namespace SchoolPortal.Application.Features.Users.Commands.RegisterUser;

public sealed record RegisterUserResult(
    Guid UserId,
    Guid MembershipId,
    string RoleName,
    string InviteToken);
