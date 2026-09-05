using MediatR;
using SchoolPortal.Application.Common.Models;

namespace SchoolPortal.Application.Features.Students.Queries.GetAllStudents;

/// <summary>Lists students in the current tenant, newest first, with optional status/search filters.</summary>
public sealed record GetAllStudentsQuery(
    int Page = 1,
    int PageSize = 25,
    string? Status = null,
    string? Search = null) : IRequest<PagedResult<StudentListItemDto>>;
