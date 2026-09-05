using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class AttendanceRecord
{
    public long Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid StudentId { get; set; }

    public Guid ClassSectionId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public string Status { get; set; } = null!;

    public Guid MarkedByStaffId { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Staff MarkedByStaff { get; set; } = null!;

    public virtual School School { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
