namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// Commits the work tracked by the repositories in the current scope. Implemented by
/// <c>SchoolPortalDbContext</c>, so the tenant-stamping / audit interceptor still runs.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
