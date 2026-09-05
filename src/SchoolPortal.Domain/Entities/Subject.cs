using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class Subject
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    public virtual School School { get; set; } = null!;

    public virtual ICollection<TimetableSlot> TimetableSlots { get; set; } = new List<TimetableSlot>();

    public virtual User? UpdatedByNavigation { get; set; }
}
