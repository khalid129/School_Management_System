using FluentAssertions;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Features.Students.Commands.CreateStudent;
using SchoolPortal.Application.Features.Students.Queries.GetStudentById;
using SchoolPortal.Application.Tests.Common;

namespace SchoolPortal.Application.Tests.Students;

public class GetStudentByIdQueryHandlerTests
{
    private static readonly Guid SchoolA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid SchoolB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private static CreateStudentCommand ValidCommand => new(
        "ADM-1", "Ayesha", "Khan", new DateOnly(2015, 1, 1), new DateOnly(2026, 1, 1));

    [Fact]
    public async Task Returns_student_for_its_own_tenant()
    {
        const string dbName = nameof(Returns_student_for_its_own_tenant);
        var write = TestContextFactory.Create(new FakeTenantContext(SchoolA), dbName);
        var id = await new CreateStudentCommandHandler(write).Handle(ValidCommand, CancellationToken.None);

        var read = TestContextFactory.Create(new FakeTenantContext(SchoolA), dbName);
        var dto = await new GetStudentByIdQueryHandler(read).Handle(new GetStudentByIdQuery(id), CancellationToken.None);

        dto.Id.Should().Be(id);
        dto.FirstName.Should().Be("Ayesha");
    }

    [Fact]
    public async Task Returns_not_found_when_queried_from_another_tenant()
    {
        const string dbName = nameof(Returns_not_found_when_queried_from_another_tenant);
        var write = TestContextFactory.Create(new FakeTenantContext(SchoolA), dbName);
        var id = await new CreateStudentCommandHandler(write).Handle(ValidCommand, CancellationToken.None);

        var otherTenant = TestContextFactory.Create(new FakeTenantContext(SchoolB), dbName);
        var act = () => new GetStudentByIdQueryHandler(otherTenant)
            .Handle(new GetStudentByIdQuery(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
