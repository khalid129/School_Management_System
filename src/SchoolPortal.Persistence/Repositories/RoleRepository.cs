using Microsoft.EntityFrameworkCore;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Domain.Entities;

namespace SchoolPortal.Persistence.Repositories;

public sealed class RoleRepository(SchoolPortalDbContext db) : IRoleRepository
{
    public Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        var normalized = (roleName ?? string.Empty).Trim().ToUpperInvariant();
        return db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == normalized, cancellationToken);
    }

    public async Task EnsureSeededAsync(IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken = default)
    {
        var missing = await MissingAsync(roleNames, cancellationToken);
        if (missing.Count == 0)
            return;

        foreach (var name in missing)
        {
            db.Roles.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                NormalizedName = name.ToUpperInvariant(),
            });
        }

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // A concurrent instance inserted the same role(s) first. Only a genuine gap is an error.
            if ((await MissingAsync(roleNames, cancellationToken)).Count > 0)
                throw;
        }
    }

    private async Task<List<string>> MissingAsync(IReadOnlyCollection<string> roleNames, CancellationToken ct)
    {
        var existing = await db.Roles.Select(r => r.NormalizedName).ToListAsync(ct);
        var have = new HashSet<string>(existing, StringComparer.Ordinal);
        return roleNames.Where(name => !have.Contains(name.ToUpperInvariant())).ToList();
    }
}
