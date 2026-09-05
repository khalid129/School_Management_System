using MediatR;

namespace SchoolPortal.Application.Features.Students.Commands.CreateStudent;

/// <summary>
/// Admits a new student into the current tenant. SchoolId is not part of the contract —
/// it is stamped from the resolved tenant by the SaveChanges interceptor.
/// </summary>
public sealed record CreateStudentCommand(
    string AdmissionNumber,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    DateOnly AdmissionDate,
    string? Gender = null,
    Guid? CurrentClassSectionId = null,
    string? RollNumber = null) : IRequest<Guid>;
