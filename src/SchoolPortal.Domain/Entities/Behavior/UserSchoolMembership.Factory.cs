using SchoolPortal.Domain.Common;

namespace SchoolPortal.Domain.Entities;

public partial class UserSchoolMembership
{
    /// <summary>
    /// Creates an invited membership linking a user to a role at the current school.
    /// SchoolId is intentionally left unset — the SaveChanges interceptor stamps it (and the
    /// audit columns) from the resolved tenant.
    /// </summary>
    public static UserSchoolMembership CreateInvited(
        Guid userId,
        Guid roleId,
        bool isPrimary,
        DateTime utcNow)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId is required.");
        if (roleId == Guid.Empty)
            throw new DomainException("RoleId is required.");

        return new UserSchoolMembership
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            Status = MembershipStatus.Invited,
            IsPrimary = isPrimary,
            InvitedOn = utcNow,
            IsActive = true,
            IsDeleted = false,
        };
    }
}
