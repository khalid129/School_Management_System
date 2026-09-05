using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Infrastructure.Tenancy;

/// <summary>
/// Resolves the current tenant per request:
///  1. the <c>school_id</c> claim on the authenticated principal (the real path, once auth ships);
///  2. in the Development environment only, the <c>X-School-Id</c> request header;
///  3. in the Development environment only, the <c>Tenancy:DevSchoolId</c> configuration value.
/// A platform super-admin is identified by a <c>platform_admin</c> claim of "true".
/// </summary>
public sealed class HttpTenantContext(
    IHttpContextAccessor httpContextAccessor,
    IHostEnvironment environment,
    IConfiguration configuration) : ITenantContext
{
    public const string SchoolIdClaimType = "school_id";
    public const string PlatformAdminClaimType = "platform_admin";
    public const string DevSchoolIdHeader = "X-School-Id";
    public const string DevSchoolIdConfigKey = "Tenancy:DevSchoolId";

    public Guid? CurrentSchoolId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            var principal = httpContext?.User;

            var fromClaim = principal?.FindFirstValue(SchoolIdClaimType);
            if (Guid.TryParse(fromClaim, out var claimSchoolId))
                return claimSchoolId;

            if (!environment.IsDevelopment())
                return null;

            var fromHeader = httpContext?.Request.Headers[DevSchoolIdHeader].ToString();
            if (Guid.TryParse(fromHeader, out var headerSchoolId))
                return headerSchoolId;

            var fromConfig = configuration[DevSchoolIdConfigKey];
            if (Guid.TryParse(fromConfig, out var configSchoolId))
                return configSchoolId;

            return null;
        }
    }

    public bool IsPlatformSuperAdmin
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(PlatformAdminClaimType);
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
