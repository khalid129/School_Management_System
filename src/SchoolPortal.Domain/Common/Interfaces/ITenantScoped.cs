namespace SchoolPortal.Domain.Common.Interfaces;

/// <summary>
/// Marker for entities that belong to exactly one tenant (school). The EF Core global
/// query filter restricts reads to the current tenant, and the SaveChanges interceptor
/// stamps <see cref="SchoolId"/> on insert and rejects cross-tenant writes.
/// Property name and type match the scaffolded entities' <c>SchoolId</c> column.
/// </summary>
public interface ITenantScoped
{
    Guid SchoolId { get; set; }
}
