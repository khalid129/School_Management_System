using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class School
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LegalName { get; set; }

    public string? Address { get; set; }

    public string City { get; set; } = null!;

    public string? CurriculumBoard { get; set; }

    public string? LogoUrl { get; set; }

    public Guid SubscriptionPlanId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? TrialEndsOn { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    // Manual correction: scaffolder mis-inferred this as a 1:1 reference (see
    // SchoolPortalDbContext.OnModelCreating for AcademicYear). It is 1:many.
    public virtual ICollection<AcademicYear> AcademicYears { get; set; } = new List<AcademicYear>();

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual ICollection<ClassLevel> ClassLevels { get; set; } = new List<ClassLevel>();

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<GuardianRelationship> GuardianRelationships { get; set; } = new List<GuardianRelationship>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();

    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    public virtual ICollection<TimetableSlot> TimetableSlots { get; set; } = new List<TimetableSlot>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserSchoolMembership> UserSchoolMemberships { get; set; } = new List<UserSchoolMembership>();
}
