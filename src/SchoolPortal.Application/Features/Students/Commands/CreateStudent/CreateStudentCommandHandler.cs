using MediatR;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Application.Features.Students.Commands.CreateStudent;

public sealed class CreateStudentCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateStudentCommand, Guid>
{
    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        // Domain factory enforces invariants (non-empty names, DOB in the past, ...).
        var student = Student.Create(
            request.AdmissionNumber,
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.AdmissionDate,
            request.Gender,
            request.CurrentClassSectionId,
            request.RollNumber);

        db.Students.Add(student);

        // SchoolId + CreatedOn/CreatedBy are set by SchoolPortalDbContext.SaveChangesAsync.
        await db.SaveChangesAsync(cancellationToken);

        return student.Id;
    }
}
