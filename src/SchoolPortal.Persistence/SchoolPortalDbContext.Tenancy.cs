using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Common.Interfaces;

namespace SchoolPortal.Persistence;

/// <summary>
/// Hand-written half of the scaffolded <see cref="SchoolPortalDbContext"/>: the
/// Application-facing contract, the multi-tenant global query filters, and the
/// audit / tenant-stamping <c>SaveChanges</c> override. This file is never regenerated.
/// </summary>
public partial class SchoolPortalDbContext : IApplicationDbContext, IUnitOfWork
{
    private readonly ITenantContext? _tenant;
    private readonly ICurrentUserService? _currentUser;
    private readonly IDateTimeProvider? _clock;

    /// <summary>DI constructor. The base scaffolded ctor (options only) is kept for design-time.</summary>
    public SchoolPortalDbContext(
        DbContextOptions<SchoolPortalDbContext> options,
        ITenantContext tenant,
        ICurrentUserService currentUser,
        IDateTimeProvider clock)
        : this(options)
    {
        _tenant = tenant;
        _currentUser = currentUser;
        _clock = clock;
    }

    // --- Values the global query filters close over. EF re-evaluates these per query
    //     because they are instance members of the context. ---

    private bool BypassTenantFilter => _tenant?.IsPlatformSuperAdmin ?? false;

    private Guid ActiveSchoolId => _tenant?.CurrentSchoolId ?? Guid.Empty;

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clr = entityType.ClrType;
            var tenantScoped = typeof(ITenantScoped).IsAssignableFrom(clr);
            var softDeletable = typeof(ISoftDeletable).IsAssignableFrom(clr);
            if (!tenantScoped && !softDeletable)
                continue;

            var e = Expression.Parameter(clr, "e");
            Expression body = Expression.Constant(true);

            if (softDeletable)
            {
                // !EF.Property<bool>(e, "IsDeleted")
                var isDeleted = EfProperty<bool>(e, nameof(ISoftDeletable.IsDeleted));
                body = Expression.AndAlso(body, Expression.Not(isDeleted));
            }

            if (tenantScoped)
            {
                // BypassTenantFilter || EF.Property<Guid>(e, "SchoolId") == ActiveSchoolId
                var bypass = Expression.Property(Expression.Constant(this), nameof(BypassTenantFilter));
                var schoolId = EfProperty<Guid>(e, nameof(ITenantScoped.SchoolId));
                var active = Expression.Property(Expression.Constant(this), nameof(ActiveSchoolId));
                var tenantMatch = Expression.OrElse(bypass, Expression.Equal(schoolId, active));
                body = Expression.AndAlso(body, tenantMatch);
            }

            modelBuilder.Entity(clr).HasQueryFilter(Expression.Lambda(body, e));
        }
    }

    private static MethodCallExpression EfProperty<T>(ParameterExpression entity, string name)
    {
        var method = typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(T));
        return Expression.Call(method, entity, Expression.Constant(name));
    }

    public override int SaveChanges()
    {
        ApplyTenantAndAuditRules();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantAndAuditRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantAndAuditRules()
    {
        var now = _clock?.UtcNow ?? DateTime.UtcNow;
        var userId = _currentUser?.UserId;
        var tenantKnown = _tenant is not null;
        var currentSchoolId = _tenant?.CurrentSchoolId;
        var isSuperAdmin = _tenant?.IsPlatformSuperAdmin ?? false;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
                continue;

            if (entry.Entity is ITenantScoped scoped && tenantKnown && !isSuperAdmin)
            {
                if (entry.State == EntityState.Added && scoped.SchoolId == Guid.Empty)
                {
                    if (currentSchoolId is null || currentSchoolId.Value == Guid.Empty)
                        throw new InvalidOperationException(
                            "Cannot persist a tenant-scoped entity: no current tenant is resolved.");
                    scoped.SchoolId = currentSchoolId.Value;
                }
                else if (currentSchoolId is not null && scoped.SchoolId != currentSchoolId.Value)
                {
                    throw new InvalidOperationException(
                        $"Cross-tenant write blocked: entity {entry.Entity.GetType().Name} " +
                        $"belongs to school {scoped.SchoolId}, current tenant is {currentSchoolId}.");
                }
            }

            if (entry.Entity is IAuditable auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedOn = now;
                    auditable.CreatedBy ??= userId;
                }
                else
                {
                    auditable.UpdatedOn = now;
                    auditable.UpdatedBy = userId;
                }
            }
        }
    }
}
