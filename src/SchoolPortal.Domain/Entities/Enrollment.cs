using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class Enrollment
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid StudentId { get; set; }

    public Guid ClassSectionId { get; set; }

    public Guid AcademicYearId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual School School { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
