using FluentAssertions;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Features.Users.Commands.RegisterUser;
using SchoolPortal.Application.Features.Users.Queries.GetUserById;
using SchoolPortal.Application.Tests.Common;
using SchoolPortal.Domain.Common;
using SchoolPortal.Domain.Entities;
using SchoolPortal.Persistence;

namespace SchoolPortal.Application.Tests.Users;

public class GetUserByIdQueryHandlerTests
{
    private static readonly Guid SchoolA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid SchoolB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly DateTime Now = new(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc);

    private static async Task<Guid> RegisterTeacherInA(string dbName)
    {
        var db = TestContextFactory.Create(new FakeTenantContext(SchoolA), dbName, clock: new FixedClock(Now));
        db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = RoleNames.Teacher, NormalizedName = "TEACHER" });
        await db.SaveChangesAsync();

        var repos = TestRepos.For(db);
        var handler = new RegisterUserCommandHandler(
            repos.Users, repos.Roles, repos.UnitOfWork,
            new FakePasswordHasher(), new FakeInviteTokenService(), new FixedClock(Now));

        var result = await handler.Handle(
            new RegisterUserCommand("teacher@example.com", "Ali Raza", "S3cure-pass", RoleNames.Teacher),
            CancellationToken.None);
        return result.UserId;
    }

    private static GetUserByIdQueryHandler HandlerFor(Guid? tenant, string dbName, out SchoolPortalDbContext db)
    {
        db = TestContextFactory.Create(new FakeTenantContext(tenant), dbName);
        return new GetUserByIdQueryHandler(new Persistence.Repositories.UserRepository(db));
    }

    [Fact]
    public async Task Returns_user_with_tenant_scoped_membership()
    {
        const string dbName = nameof(Returns_user_with_tenant_scoped_membership);
        var userId = await RegisterTeacherInA(dbName);

        var handler = HandlerFor(SchoolA, dbName, out _);
        var dto = await handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        dto.Id.Should().Be(userId);
        dto.Email.Should().Be("teacher@example.com");
        dto.EmailConfirmed.Should().BeFalse();
        dto.Memberships.Should().ContainSingle(m =>
            m.RoleName == RoleNames.Teacher && m.Status == MembershipStatus.Invited && m.IsPrimary);
    }

    [Fact]
    public async Task Returns_NotFound_from_another_tenant()
    {
        const string dbName = nameof(Returns_NotFound_from_another_tenant);
        var userId = await RegisterTeacherInA(dbName);

        var handler = HandlerFor(SchoolB, dbName, out _);
        var act = () => handler.Handle(new GetUserByIdQuery(userId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
