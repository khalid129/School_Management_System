using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Application.Features.Students.Queries.GetStudentById;

public sealed class GetStudentByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        // The global query filter scopes this to the current tenant, so a student in
        // another school is simply "not found" — the isolation guarantee.
        var dto = await db.Students
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .ProjectToType<StudentDto>()
            .FirstOrDefaultAsync(cancellationToken);

        return dto ?? throw new NotFoundException(nameof(Domain.Entities.Student), request.Id);
    }
}
