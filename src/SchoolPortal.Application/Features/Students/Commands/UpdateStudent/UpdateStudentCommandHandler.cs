using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Application.Features.Students.Commands.UpdateStudent;

public sealed class UpdateStudentCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateStudentCommand>
{
    public async Task Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        // Tracked load, tenant-filtered: a student outside the current school is not found.
        var student = await db.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Student), request.Id);

        // Guarded mutation via the domain method.
        student.UpdateProfile(request.FirstName, request.LastName, request.Gender, request.RollNumber);

        // UpdatedOn/UpdatedBy are stamped by SchoolPortalDbContext.SaveChangesAsync.
        await db.SaveChangesAsync(cancellationToken);
    }
}
