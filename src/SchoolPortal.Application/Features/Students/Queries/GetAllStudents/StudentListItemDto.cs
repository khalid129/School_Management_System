namespace SchoolPortal.Application.Features.Students.Queries.GetAllStudents;

public sealed record StudentListItemDto(
    Guid Id,
    string AdmissionNumber,
    string FirstName,
    string LastName,
    string Status,
    Guid? CurrentClassSectionId,
    string? RollNumber);
