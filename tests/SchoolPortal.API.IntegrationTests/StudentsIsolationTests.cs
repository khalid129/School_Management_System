using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace SchoolPortal.API.IntegrationTests;

/// <summary>
/// Exercises tenant isolation against the real configured SchoolSaaS database. Requires the
/// two seeded tenants (Test School A / B) to exist — see the plan's verification section.
/// Data is created with a random admission number so re-runs don't collide.
/// </summary>
public class StudentsIsolationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string SchoolA = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
    private const string SchoolB = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

    private readonly WebApplicationFactory<Program> _factory;

    public StudentsIsolationTests(WebApplicationFactory<Program> factory)
        => _factory = factory.WithWebHostBuilder(b => b.UseEnvironment("Development"));

    private HttpClient ClientFor(string schoolId)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-School-Id", schoolId);
        return client;
    }

    [Fact]
    public async Task Student_created_in_school_A_is_not_visible_to_school_B()
    {
        var admissionNumber = "IT-" + Guid.NewGuid().ToString("N")[..10];
        var body = new
        {
            admissionNumber,
            firstName = "Isolation",
            lastName = "Test",
            dateOfBirth = "2015-01-01",
            admissionDate = "2026-01-01",
        };

        var create = await ClientFor(SchoolA).PostAsJsonAsync("/api/v1/students", body);
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CreatedResponse>();
        created!.Id.Should().NotBeEmpty();

        var fromA = await ClientFor(SchoolA).GetAsync($"/api/v1/students/{created.Id}");
        fromA.StatusCode.Should().Be(HttpStatusCode.OK);

        var fromB = await ClientFor(SchoolB).GetAsync($"/api/v1/students/{created.Id}");
        fromB.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_without_tenant_header_is_rejected()
    {
        var client = _factory.CreateClient();
        var body = new
        {
            admissionNumber = "IT-" + Guid.NewGuid().ToString("N")[..10],
            firstName = "No",
            lastName = "Tenant",
            dateOfBirth = "2015-01-01",
            admissionDate = "2026-01-01",
        };

        var response = await client.PostAsJsonAsync("/api/v1/students", body);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private sealed record CreatedResponse(Guid Id);
}
