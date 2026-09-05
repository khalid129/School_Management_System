namespace SchoolPortal.Application.Features.Students.Queries.GetStudentById;

public sealed record StudentDto(
    Guid Id,
    string AdmissionNumber,
    string FirstName,
    string LastName,
    string? Gender,
    DateOnly DateOfBirth,
    DateOnly AdmissionDate,
    Guid? CurrentClassSectionId,
    string? RollNumber,
    string Status,
    string? PhotoUrl);
