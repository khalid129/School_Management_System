namespace SchoolPortal.Application.Features.Users.Queries.GetUserById;

public sealed record UserDto(
    Guid Id,
    string? Email,
    string FullName,
    string PreferredLanguage,
    bool IsActive,
    bool EmailConfirmed,
    IReadOnlyList<UserMembershipDto> Memberships);

public sealed record UserMembershipDto(
    Guid MembershipId,
    string RoleName,
    string Status,
    bool IsPrimary);
