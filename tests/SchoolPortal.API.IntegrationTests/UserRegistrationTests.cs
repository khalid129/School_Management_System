using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SchoolPortal.API.IntegrationTests;

/// <summary>
/// Register-user flow against the real configured SchoolSaaS database. Requires the seeded
/// tenant "Test School A". Startup seeds the 6 ROLES rows. Emails are randomised so re-runs
/// don't collide.
/// </summary>
public class UserRegistrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string SchoolA = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

    private readonly WebApplicationFactory<Program> _factory;

    public UserRegistrationTests(WebApplicationFactory<Program> factory)
        => _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    private HttpClient ClientForA()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-School-Id", SchoolA);
        return client;
    }

    private static object Body(string email, string role = "Teacher") => new
    {
        email,
        fullName = "Integration User",
        password = "S3cure-pass",
        roleName = role,
    };

    [Fact]
    public async Task Register_creates_user_and_invited_membership_then_readable()
    {
        var email = $"it-{Guid.NewGuid():N}@example.com";

        var create = await ClientForA().PostAsJsonAsync("/api/v1/users", Body(email));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await create.Content.ReadFromJsonAsync<RegisterResponse>();
        result!.UserId.Should().NotBeEmpty();
        result.MembershipId.Should().NotBeEmpty();
        result.RoleName.Should().Be("Teacher");
        result.InviteToken.Should().NotBeNullOrWhiteSpace();

        var get = await ClientForA().GetAsync($"/api/v1/users/{result.UserId}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await get.Content.ReadFromJsonAsync<UserResponse>();
        dto!.Email.Should().Be(email);
        dto.EmailConfirmed.Should().BeFalse();
        dto.Memberships.Should().ContainSingle(m => m.RoleName == "Teacher" && m.Status == "Invited");
    }

    [Fact]
    public async Task Register_rejects_duplicate_email()
    {
        var email = $"it-{Guid.NewGuid():N}@example.com";
        (await ClientForA().PostAsJsonAsync("/api/v1/users", Body(email))).EnsureSuccessStatusCode();

        var again = await ClientForA().PostAsJsonAsync("/api/v1/users", Body(email));
        again.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_rejects_unknown_role()
    {
        var res = await ClientForA().PostAsJsonAsync(
            "/api/v1/users", Body($"it-{Guid.NewGuid():N}@example.com", role: "Wizard"));
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_without_tenant_header_is_rejected()
    {
        var res = await _factory.CreateClient().PostAsJsonAsync(
            "/api/v1/users", Body($"it-{Guid.NewGuid():N}@example.com"));
        res.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private sealed record RegisterResponse(Guid UserId, Guid MembershipId, string RoleName, string InviteToken);
    private sealed record UserResponse(Guid Id, string Email, bool EmailConfirmed, MembershipResponse[] Memberships);
    private sealed record MembershipResponse(Guid MembershipId, string RoleName, string Status, bool IsPrimary);
}
