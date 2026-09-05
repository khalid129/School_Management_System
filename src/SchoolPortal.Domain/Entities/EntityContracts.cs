using SchoolPortal.Domain.Common.Interfaces;

namespace SchoolPortal.Domain.Entities;

// Attaches cross-cutting marker interfaces to the scaffolded (partial) entity classes.
// Kept in one hand-maintained file so `dotnet ef dbcontext scaffold --force` (which only
// rewrites the per-entity files it generates) never clobbers it. See tools/rescaffold.ps1.
//
// Groups:
//   ITenantScoped  -> has a SCHOOL_ID column (tenant-filtered + auto-stamped)
//   IAuditable     -> has CREATED_ON/BY + UPDATED_ON/BY
//   ISoftDeletable -> has IS_DELETED (row hidden by the global query filter)
//
// Global / not attached: User, Role, PlatformAdministrator, RefreshToken (Identity & Auth
// layer — no SCHOOL_ID, no full audit set). School is audited + soft-deletable but is the
// tenant root itself, so it is NOT ITenantScoped.

public partial class School : IAuditable, ISoftDeletable { }
public partial class SubscriptionPlan : IAuditable, ISoftDeletable { }
public partial class SubscriptionPlanFeature : IAuditable, ISoftDeletable { }

public partial class UserSchoolMembership : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class AcademicYear : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class ClassLevel : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class ClassSection : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class Staff : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class Student : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class GuardianRelationship : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class Enrollment : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class AttendanceRecord : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class Subject : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class TimetableSlot : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class Exam : ITenantScoped, IAuditable, ISoftDeletable { }
public partial class ExamResult : ITenantScoped, IAuditable, ISoftDeletable { }
