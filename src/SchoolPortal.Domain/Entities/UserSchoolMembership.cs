using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class UserSchoolMembership
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid SchoolId { get; set; }

    public Guid RoleId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public DateTime? InvitedOn { get; set; }

    public DateTime? JoinedOn { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual School School { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
