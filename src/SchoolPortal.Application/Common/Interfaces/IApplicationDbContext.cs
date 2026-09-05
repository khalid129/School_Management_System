using Microsoft.EntityFrameworkCore;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// The Application layer's view of the database. Implemented by
/// <c>SchoolPortalDbContext</c> in the Persistence layer. Every tenant-scoped
/// <see cref="DbSet{T}"/> is already filtered to the current tenant by the context's
/// global query filters, so handlers query freely without repeating a SchoolId predicate.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<AcademicYear> AcademicYears { get; }
    DbSet<AttendanceRecord> AttendanceRecords { get; }
    DbSet<ClassLevel> ClassLevels { get; }
    DbSet<ClassSection> ClassSections { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<Exam> Exams { get; }
    DbSet<ExamResult> ExamResults { get; }
    DbSet<GuardianRelationship> GuardianRelationships { get; }
    DbSet<PlatformAdministrator> PlatformAdministrators { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Role> Roles { get; }
    DbSet<School> Schools { get; }
    DbSet<Staff> Staff { get; }
    DbSet<Student> Students { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; }
    DbSet<TimetableSlot> TimetableSlots { get; }
    DbSet<User> Users { get; }
    DbSet<UserSchoolMembership> UserSchoolMemberships { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
