using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Persistence;
using SchoolPortal.Persistence.Repositories;

namespace SchoolPortal.Application.Tests.Common;

/// <summary>
/// Fakes for the tenant/user/clock services plus a helper to build a <see cref="SchoolPortalDbContext"/>
/// on the EF in-memory provider. In-memory ignores the SQL-Server-specific relational model
/// (indexes, defaults, filtered indexes) but still applies global query filters and the
/// SaveChanges override — which is exactly what these tests exercise.
/// </summary>
public sealed class FakeTenantContext(Guid? schoolId, bool superAdmin = false) : ITenantContext
{
    public Guid? CurrentSchoolId { get; set; } = schoolId;
    public bool IsPlatformSuperAdmin { get; } = superAdmin;
}

public sealed class FakeCurrentUser(Guid? userId = null) : ICurrentUserService
{
    public Guid? UserId { get; } = userId;
}

public sealed class FixedClock(DateTime utcNow) : IDateTimeProvider
{
    public DateTime UtcNow { get; } = utcNow;
}

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed::{password}";
    public bool Verify(string hash, string password) => hash == $"hashed::{password}";
}

public sealed class FakeInviteTokenService : IInviteTokenService
{
    public string Create(Guid userId, string securityStamp) => $"invite::{userId:N}::{securityStamp}";

    public bool TryValidate(string token, string securityStamp, out Guid userId)
    {
        userId = Guid.Empty;
        var parts = token.Split("::");
        return parts.Length == 3 && parts[0] == "invite"
            && parts[2] == securityStamp && Guid.TryParseExact(parts[1], "N", out userId);
    }
}

public static class TestContextFactory
{
    public static SchoolPortalDbContext Create(
        ITenantContext tenant,
        string? databaseName = null,
        ICurrentUserService? user = null,
        IDateTimeProvider? clock = null)
    {
        var options = new DbContextOptionsBuilder<SchoolPortalDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        return new SchoolPortalDbContext(
            options,
            tenant,
            user ?? new FakeCurrentUser(),
            clock ?? new FixedClock(DateTime.UtcNow));
    }
}

/// <summary>Real repository implementations over an in-memory context, for handler tests.</summary>
public sealed record TestRepos(IUserRepository Users, IRoleRepository Roles, IUnitOfWork UnitOfWork)
{
    public static TestRepos For(SchoolPortalDbContext ctx)
        => new(new UserRepository(ctx), new RoleRepository(ctx), ctx);
}
