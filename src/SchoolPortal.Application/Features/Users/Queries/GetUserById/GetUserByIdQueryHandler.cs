using MediatR;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository users)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Memberships are tenant-scoped by the global query filter. A user with no membership
        // in the current tenant is "not found" here — the isolation guarantee.
        var memberships = await users.ListMembershipsWithRoleAsync(request.Id, cancellationToken);
        if (memberships.Count == 0)
            throw new NotFoundException(nameof(Domain.Entities.User), request.Id);

        var user = await users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), request.Id);

        return new UserDto(
            user.Id,
            user.Email,
            user.FullName,
            user.PreferredLanguage,
            user.IsActive,
            user.EmailConfirmed,
            memberships
                .Select(m => new UserMembershipDto(m.Id, m.Role.Name, m.Status, m.IsPrimary))
                .ToList());
    }
}
