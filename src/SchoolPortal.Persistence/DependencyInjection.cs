using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Persistence.Repositories;

namespace SchoolPortal.Persistence;

public static class DependencyInjection
{
    public const string ConnectionStringName = "SchoolSaaS";

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is not configured.");

        services.AddDbContext<SchoolPortalDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        // Legacy surface still used by the Students slice (to be removed once it moves to repositories).
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<SchoolPortalDbContext>());

        // Repository pattern: handlers depend on these, never on the DbContext.
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SchoolPortalDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        return services;
    }
}
