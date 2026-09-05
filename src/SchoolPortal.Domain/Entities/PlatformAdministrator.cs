using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class PlatformAdministrator
{
    public Guid UserId { get; set; }

    public DateTime GrantedOn { get; set; }

    public Guid? GrantedBy { get; set; }

    public bool IsActive { get; set; }

    public virtual User? GrantedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
