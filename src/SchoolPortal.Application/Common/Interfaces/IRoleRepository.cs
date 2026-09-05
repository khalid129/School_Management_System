using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Application.Common.Interfaces;

public interface IRoleRepository
{
    /// <summary>Looks up a role by its display name (matched case-insensitively). Null if not found.</summary>
    Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken);

    /// <summary>
    /// Idempotently ensures every named role exists. Safe on every startup and safe when
    /// several app instances start at once (the unique index on NORMALIZED_NAME is the tie-breaker).
    /// </summary>
    Task EnsureSeededAsync(IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken = default);
}
