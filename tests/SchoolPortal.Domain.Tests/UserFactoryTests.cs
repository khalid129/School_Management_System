using FluentAssertions;
using SchoolPortal.Domain.Common;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Domain.Tests;

public class UserFactoryTests
{
    private static readonly DateTime Now = new(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc);

    private static User Valid() =>
        User.Create("Teacher@Example.com", "Ali Raza", "en", "HASH", "+92-300", Now);

    [Fact]
    public void Create_normalizes_email_and_sets_identity_fields()
    {
        var user = Valid();

        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be("Teacher@Example.com");
        user.NormalizedEmail.Should().Be("TEACHER@EXAMPLE.COM");
        user.UserName.Should().Be("Teacher@Example.com");
        user.NormalizedUserName.Should().Be("TEACHER@EXAMPLE.COM");
        user.PasswordHash.Should().Be("HASH");
        user.SecurityStamp.Should().NotBeNullOrWhiteSpace();
        user.ConcurrencyStamp.Should().NotBeNullOrWhiteSpace();
        user.EmailConfirmed.Should().BeFalse();
        user.IsActive.Should().BeTrue();
        user.CreatedOn.Should().Be(Now);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign")]
    public void Create_rejects_invalid_email(string email)
    {
        var act = () => User.Create(email, "Ali", "en", "HASH", null, Now);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_rejects_blank_name()
    {
        var act = () => User.Create("a@b.com", "  ", "en", "HASH", null, Now);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_rejects_empty_password_hash()
    {
        var act = () => User.Create("a@b.com", "Ali", "en", "", null, Now);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_defaults_unknown_language_to_en()
    {
        User.Create("a@b.com", "Ali", "fr", "HASH", null, Now).PreferredLanguage.Should().Be("en");
        User.Create("a@b.com", "Ali", null, "HASH", null, Now).PreferredLanguage.Should().Be("en");
        User.Create("a@b.com", "Ali", "ur", "HASH", null, Now).PreferredLanguage.Should().Be("ur");
    }

    [Fact]
    public void CreateInvited_membership_starts_invited_without_tenant()
    {
        var m = UserSchoolMembership.CreateInvited(Guid.NewGuid(), Guid.NewGuid(), isPrimary: true, Now);

        m.Status.Should().Be(MembershipStatus.Invited);
        m.IsPrimary.Should().BeTrue();
        m.InvitedOn.Should().Be(Now);
        m.SchoolId.Should().Be(Guid.Empty, "the SaveChanges interceptor stamps the tenant");
    }
}
