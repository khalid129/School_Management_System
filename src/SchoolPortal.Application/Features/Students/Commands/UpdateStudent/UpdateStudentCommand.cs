using MediatR;

namespace SchoolPortal.Application.Features.Students.Commands.UpdateStudent;

/// <summary>Updates a student's mutable profile fields within the current tenant.</summary>
public sealed record UpdateStudentCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Gender = null,
    string? RollNumber = null) : IRequest;
