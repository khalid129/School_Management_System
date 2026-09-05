namespace SchoolPortal.Domain.Common;

/// <summary>
/// The institute-scoped roles seeded into <c>ROLES</c> and assigned via
/// <c>USER_SCHOOL_MEMBERSHIPS.ROLE_ID</c>. Names match DB-Structure-Institute-Portal.md.
/// </summary>
public static class RoleNames
{
    public const string SchoolAdmin = "SchoolAdmin";
    public const string Teacher = "Teacher";
    public const string Accountant = "Accountant";
    public const string HrStaff = "HRStaff";
    public const string Parent = "Parent";
    public const string Student = "Student";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SchoolAdmin, Teacher, Accountant, HrStaff, Parent, Student,
    };
}
