using FluentAssertions;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Features.Users.Commands.RegisterUser;
using SchoolPortal.Application.Tests.Common;
using SchoolPortal.Domain.Common;
using SchoolPortal.Domain.Entities;
using SchoolPortal.Persistence;

namespace SchoolPortal.Application.Tests.Users;

public class RegisterUserCommandHandlerTests
{
    private static readonly Guid SchoolA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly DateTime Now = new(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc);

    private static RegisterUserCommand Command(string email = "teacher@example.com", string role = RoleNames.Teacher)
        => new(email, "Ali Raza", "S3cure-pass", role);

    private static async Task<SchoolPortalDbContext> SeededContext(string dbName, Guid? schoolId)
    {
        var db = TestContextFactory.Create(new FakeTenantContext(schoolId), dbName, clock: new FixedClock(Now));
        db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = RoleNames.Teacher, NormalizedName = "TEACHER" });
        await db.SaveChangesAsync();
        return db;
    }

    private static RegisterUserCommandHandler Handler(SchoolPortalDbContext db)
    {
        var repos = TestRepos.For(db);
        return new RegisterUserCommandHandler(
            repos.Users, repos.Roles, repos.UnitOfWork,
            new FakePasswordHasher(), new FakeInviteTokenService(), new FixedClock(Now));
    }

    [Fact]
    public async Task Handle_creates_user_and_invited_membership_stamped_to_current_tenant()
    {
        const string dbName = nameof(Handle_creates_user_and_invited_membership_stamped_to_current_tenant);
        var db = await SeededContext(dbName, SchoolA);

        var result = await Handler(db).Handle(Command(), CancellationToken.None);

        result.UserId.Should().NotBeEmpty();
        result.MembershipId.Should().NotBeEmpty();
        result.RoleName.Should().Be(RoleNames.Teacher);
        result.InviteToken.Should().NotBeNullOrWhiteSpace();

        var read = TestContextFactory.Create(new FakeTenantContext(SchoolA), dbName);
        var user = await read.Users.FindAsync(result.UserId);
        user.Should().NotBeNull();
        user!.PasswordHash.Should().Be("hashed::S3cure-pass");
        user.NormalizedEmail.Should().Be("TEACHER@EXAMPLE.COM");
        user.EmailConfirmed.Should().BeFalse();

        var membership = await read.UserSchoolMemberships.FindAsync(result.MembershipId);
        membership.Should().NotBeNull();
        membership!.SchoolId.Should().Be(SchoolA, "the interceptor stamps SchoolId from ITenantContext");
        membership.Status.Should().Be(MembershipStatus.Invited);
        membership.UserId.Should().Be(result.UserId);
    }

    [Fact]
    public async Task Handle_throws_NotFound_for_unknown_role()
    {
        const string dbName = nameof(Handle_throws_NotFound_for_unknown_role);
        var db = await SeededContext(dbName, SchoolA);

        var act = () => Handler(db).Handle(Command(role: "Wizard"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_throws_when_no_tenant_is_resolved()
    {
        const string dbName = nameof(Handle_throws_when_no_tenant_is_resolved);
        var db = await SeededContext(dbName, schoolId: null);

        var act = () => Handler(db).Handle(Command(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*tenant*");
    }
}
