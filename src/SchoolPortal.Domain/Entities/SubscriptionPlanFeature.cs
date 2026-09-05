using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class SubscriptionPlanFeature
{
    public Guid Id { get; set; }

    public Guid SubscriptionPlanId { get; set; }

    public string FeatureKey { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
