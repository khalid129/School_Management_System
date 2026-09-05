using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class Student
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid? ApplicationUserId { get; set; }

    public string AdmissionNumber { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public DateOnly AdmissionDate { get; set; }

    public Guid? CurrentClassSectionId { get; set; }

    public string? RollNumber { get; set; }

    public string Status { get; set; } = null!;

    public string? PhotoUrl { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? ApplicationUser { get; set; }

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ClassSection? CurrentClassSection { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual ICollection<GuardianRelationship> GuardianRelationships { get; set; } = new List<GuardianRelationship>();

    public virtual School School { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
