using System;
using System.Collections.Generic;

namespace SchoolPortal.Domain.Entities;

public partial class SubscriptionPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int StudentCountMin { get; set; }

    public int? StudentCountMax { get; set; }

    public decimal MonthlyPriceAmount { get; set; }

    public string Currency { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<School> Schools { get; set; } = new List<School>();

    public virtual ICollection<SubscriptionPlanFeature> SubscriptionPlanFeatures { get; set; } = new List<SubscriptionPlanFeature>();

    public virtual User? UpdatedByNavigation { get; set; }
}
