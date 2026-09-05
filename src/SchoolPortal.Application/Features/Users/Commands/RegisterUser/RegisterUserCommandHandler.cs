using MediatR;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Application.Features.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IRoleRepository roles,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IInviteTokenService inviteTokens,
    IDateTimeProvider clock) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var role = await roles.GetByNameAsync(request.RoleName, cancellationToken)
            ?? throw new NotFoundException($"Role \"{request.RoleName}\" was not found. Has role seeding run?");

        var now = clock.UtcNow;

        var user = User.Create(
            request.Email,
            request.FullName,
            request.PreferredLanguage,
            passwordHasher.Hash(request.Password),
            request.PhoneNumber,
            now);

        var membership = UserSchoolMembership.CreateInvited(user.Id, role.Id, request.IsPrimary, now);

        users.Add(user);
        users.AddMembership(membership);

        // One SaveChanges = one transaction. The interceptor stamps membership.SchoolId from
        // the tenant (and throws 409-mapped InvalidOperationException if none is resolved).
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var inviteToken = inviteTokens.Create(user.Id, user.SecurityStamp!);

        return new RegisterUserResult(user.Id, membership.Id, role.Name, inviteToken);
    }
}
