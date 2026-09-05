using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class GuardianRelationship
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public Guid StudentId { get; set; }

    public Guid GuardianUserId { get; set; }

    public string RelationshipType { get; set; } = null!;

    public bool IsPrimaryContact { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User GuardianUser { get; set; } = null!;

    public virtual School School { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
