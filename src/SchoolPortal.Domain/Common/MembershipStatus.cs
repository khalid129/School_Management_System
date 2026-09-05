namespace SchoolPortal.Domain.Common;

/// <summary>Lifecycle of a <c>USER_SCHOOL_MEMBERSHIPS</c> row (its STATUS column).</summary>
public static class MembershipStatus
{
    public const string Invited = "Invited";
    public const string Active = "Active";
    public const string Suspended = "Suspended";
}
