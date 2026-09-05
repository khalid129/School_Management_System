using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class TimetableSlot
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid ClassSectionId { get; set; }

    public Guid SubjectId { get; set; }

    public Guid StaffId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? RoomName { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual User? CreatedByNavigation { get; set; }

    public virtual School School { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
