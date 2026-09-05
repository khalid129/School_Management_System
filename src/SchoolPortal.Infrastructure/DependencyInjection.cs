using Microsoft.Extensions.DependencyInjection;
using SchoolPortal.Application.Common.Interfaces;
using SchoolPortal.Infrastructure.Identity;
using SchoolPortal.Infrastructure.Tenancy;
using SchoolPortal.Infrastructure.Time;

namespace SchoolPortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ITenantContext, HttpTenantContext>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<IInviteTokenService, InviteTokenService>();

        return services;
    }
}
