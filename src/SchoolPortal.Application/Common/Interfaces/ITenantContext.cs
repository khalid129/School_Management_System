namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// Ambient information about the tenant (school) the current request acts for. Resolved
/// per request from the <c>school_id</c> JWT claim once auth ships; until then, in
/// Development, from the <c>X-School-Id</c> header (see Infrastructure.Tenancy).
/// </summary>
public interface ITenantContext
{
    /// <summary>The current tenant, or null when unresolved (unauthenticated / platform scope).</summary>
    Guid? CurrentSchoolId { get; }

    /// <summary>True for platform super-admins, who bypass the tenant query filter.</summary>
    bool IsPlatformSuperAdmin { get; }
}
