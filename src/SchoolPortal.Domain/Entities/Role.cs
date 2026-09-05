using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;

    public virtual ICollection<UserSchoolMembership> UserSchoolMemberships { get; set; } = new List<UserSchoolMembership>();
}
