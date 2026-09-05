using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class AcademicYear
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsCurrent { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual School School { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
