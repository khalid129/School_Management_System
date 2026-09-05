using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class Exam
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid AcademicYearId { get; set; }

    public Guid ClassLevelId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly ExamStartDate { get; set; }

    public DateOnly ExamEndDate { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual AcademicYear AcademicYear { get; set; } = null!;

    public virtual ClassLevel ClassLevel { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual School School { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
