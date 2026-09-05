using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Application.Common.Models;

namespace SchoolPortal.Application.Features.Students.Queries.GetAllStudents;

public sealed class GetAllStudentsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllStudentsQuery, PagedResult<StudentListItemDto>>
{
    public async Task<PagedResult<StudentListItemDto>> Handle(
        GetAllStudentsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 25 : request.PageSize;

        var query = db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(s => s.Status == request.Status);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(s =>
                s.FirstName.Contains(term) ||
                s.LastName.Contains(term) ||
                s.AdmissionNumber.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectToType<StudentListItemDto>()
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentListItemDto>(items, page, pageSize, totalCount);
    }
}
