using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class ClassSection
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid ClassLevelId { get; set; }

    public Guid AcademicYearId { get; set; }

    public string Name { get; set; } = null!;

    public Guid? ClassTeacherStaffId { get; set; }

    public int? Capacity { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

    public virtual ClassLevel ClassLevel { get; set; } = null!;

    public virtual Staff? ClassTeacherStaff { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual School School { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<TimetableSlot> TimetableSlots { get; set; } = new List<TimetableSlot>();

    public virtual User? UpdatedByNavigation { get; set; }
}
