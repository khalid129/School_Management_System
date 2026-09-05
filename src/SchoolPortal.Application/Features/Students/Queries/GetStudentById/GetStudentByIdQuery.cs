using MediatR;

namespace SchoolPortal.Application.Features.Students.Queries.GetStudentById;

public sealed record GetStudentByIdQuery(Guid Id) : IRequest<StudentDto>;
