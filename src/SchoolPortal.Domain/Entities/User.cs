using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public bool LockoutEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public string FullName { get; set; } = null!;

    public string PreferredLanguage { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<AcademicYear> AcademicYearCreatedByNavigations { get; set; } = new List<AcademicYear>();

    public virtual ICollection<AcademicYear> AcademicYearUpdatedByNavigations { get; set; } = new List<AcademicYear>();

    public virtual ICollection<AttendanceRecord> AttendanceRecordCreatedByNavigations { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<AttendanceRecord> AttendanceRecordUpdatedByNavigations { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<ClassLevel> ClassLevelCreatedByNavigations { get; set; } = new List<ClassLevel>();

    public virtual ICollection<ClassLevel> ClassLevelUpdatedByNavigations { get; set; } = new List<ClassLevel>();

    public virtual ICollection<ClassSection> ClassSectionCreatedByNavigations { get; set; } = new List<ClassSection>();

    public virtual ICollection<ClassSection> ClassSectionUpdatedByNavigations { get; set; } = new List<ClassSection>();

    public virtual ICollection<Enrollment> EnrollmentCreatedByNavigations { get; set; } = new List<Enrollment>();

    public virtual ICollection<Enrollment> EnrollmentUpdatedByNavigations { get; set; } = new List<Enrollment>();

    public virtual ICollection<Exam> ExamCreatedByNavigations { get; set; } = new List<Exam>();

    public virtual ICollection<ExamResult> ExamResultCreatedByNavigations { get; set; } = new List<ExamResult>();

    public virtual ICollection<ExamResult> ExamResultUpdatedByNavigations { get; set; } = new List<ExamResult>();

    public virtual ICollection<Exam> ExamUpdatedByNavigations { get; set; } = new List<Exam>();

    public virtual ICollection<GuardianRelationship> GuardianRelationshipCreatedByNavigations { get; set; } = new List<GuardianRelationship>();

    public virtual ICollection<GuardianRelationship> GuardianRelationshipGuardianUsers { get; set; } = new List<GuardianRelationship>();

    public virtual ICollection<GuardianRelationship> GuardianRelationshipUpdatedByNavigations { get; set; } = new List<GuardianRelationship>();

    public virtual ICollection<PlatformAdministrator> PlatformAdministratorGrantedByNavigations { get; set; } = new List<PlatformAdministrator>();

    public virtual PlatformAdministrator? PlatformAdministratorUser { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<School> SchoolCreatedByNavigations { get; set; } = new List<School>();

    public virtual ICollection<School> SchoolUpdatedByNavigations { get; set; } = new List<School>();

    public virtual ICollection<Staff> StaffApplicationUsers { get; set; } = new List<Staff>();

    public virtual ICollection<Staff> StaffCreatedByNavigations { get; set; } = new List<Staff>();

    public virtual ICollection<Staff> StaffUpdatedByNavigations { get; set; } = new List<Staff>();

    public virtual ICollection<Student> StudentApplicationUsers { get; set; } = new List<Student>();

    public virtual ICollection<Student> StudentCreatedByNavigations { get; set; } = new List<Student>();

    public virtual ICollection<Student> StudentUpdatedByNavigations { get; set; } = new List<Student>();

    public virtual ICollection<Subject> SubjectCreatedByNavigations { get; set; } = new List<Subject>();

    public virtual ICollection<Subject> SubjectUpdatedByNavigations { get; set; } = new List<Subject>();

    public virtual ICollection<SubscriptionPlan> SubscriptionPlanCreatedByNavigations { get; set; } = new List<SubscriptionPlan>();

    public virtual ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatureCreatedByNavigations { get; set; } = new List<SubscriptionPlanFeature>();

    public virtual ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatureUpdatedByNavigations { get; set; } = new List<SubscriptionPlanFeature>();

    public virtual ICollection<SubscriptionPlan> SubscriptionPlanUpdatedByNavigations { get; set; } = new List<SubscriptionPlan>();

    public virtual ICollection<TimetableSlot> TimetableSlotCreatedByNavigations { get; set; } = new List<TimetableSlot>();

    public virtual ICollection<TimetableSlot> TimetableSlotUpdatedByNavigations { get; set; } = new List<TimetableSlot>();

    public virtual ICollection<UserSchoolMembership> UserSchoolMembershipCreatedByNavigations { get; set; } = new List<UserSchoolMembership>();

    public virtual ICollection<UserSchoolMembership> UserSchoolMembershipUpdatedByNavigations { get; set; } = new List<UserSchoolMembership>();

    public virtual ICollection<UserSchoolMembership> UserSchoolMembershipUsers { get; set; } = new List<UserSchoolMembership>();
}
