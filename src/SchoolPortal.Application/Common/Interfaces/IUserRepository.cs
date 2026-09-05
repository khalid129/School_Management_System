using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Application.Common.Interfaces;

/// <summary>
/// Data access for the User aggregate (the global USERS row plus its school memberships).
/// Handlers depend on this — never on the DbContext. Writes are staged by <see cref="Add"/> /
/// <see cref="AddMembership"/> and committed via <see cref="IUnitOfWork.SaveChangesAsync"/>.
/// </summary>
public interface IUserRepository
{
    /// <summary>True if any account already uses this normalized email (platform-wide — USERS is global).</summary>
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>The user's memberships in the current tenant, each with its <see cref="Role"/> loaded.</summary>
    Task<IReadOnlyList<UserSchoolMembership>> ListMembershipsWithRoleAsync(Guid userId, CancellationToken cancellationToken);

    void Add(User user);

    void AddMembership(UserSchoolMembership membership);
}
