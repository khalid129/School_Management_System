using FluentAssertions;
using SchoolPortal.Application.Features.Students.Commands.CreateStudent;
using SchoolPortal.Application.Tests.Common;

namespace SchoolPortal.Application.Tests.Students;

public class CreateStudentCommandHandlerTests
{
    private static readonly Guid SchoolA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static CreateStudentCommand ValidCommand(string admissionNumber = "ADM-1") => new(
        admissionNumber,
        "Ayesha",
        "Khan",
        new DateOnly(2015, 1, 1),
        new DateOnly(2026, 1, 1));

    [Fact]
    public async Task Handle_stamps_current_tenant_and_audit_columns_on_insert()
    {
        var db = TestContextFactory.Create(
            new FakeTenantContext(SchoolA),
            clock: new FixedClock(new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc)));
        var handler = new CreateStudentCommandHandler(db);

        var id = await handler.Handle(ValidCommand(), CancellationToken.None);

        var saved = await db.Students.FindAsync(id);
        saved.Should().NotBeNull();
        saved!.SchoolId.Should().Be(SchoolA, "the interceptor stamps SchoolId from ITenantContext");
        saved.CreatedOn.Should().Be(new DateTime(2026, 9, 5, 12, 0, 0, DateTimeKind.Utc));
        saved.Status.Should().Be("Active");
    }

    [Fact]
    public async Task Handle_throws_when_no_tenant_is_resolved()
    {
        var db = TestContextFactory.Create(new FakeTenantContext(schoolId: null));
        var handler = new CreateStudentCommandHandler(db);

        var act = () => handler.Handle(ValidCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*tenant*");
    }
}
