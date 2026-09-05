using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Persistence.Repositories;

public sealed class UserRepository(SchoolPortalDbContext db) : IUserRepository
{
    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
        => db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<IReadOnlyList<UserSchoolMembership>> ListMembershipsWithRoleAsync(
        Guid userId, CancellationToken cancellationToken)
        => await db.UserSchoolMemberships
            .AsNoTracking()
            .Include(m => m.Role)
            .Where(m => m.UserId == userId) // tenant scope applied by the global query filter
            .ToListAsync(cancellationToken);

    public void Add(User user) => db.Users.Add(user);

    public void AddMembership(UserSchoolMembership membership) => db.UserSchoolMemberships.Add(membership);
}
