using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class ExamResult
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid ExamId { get; set; }

    public Guid StudentId { get; set; }

    public Guid SubjectId { get; set; }

    public decimal? MarksObtained { get; set; }

    public decimal MaxMarks { get; set; }

    public string? Grade { get; set; }

    public Guid? EnteredByStaffId { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Staff? EnteredByStaff { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual School School { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
