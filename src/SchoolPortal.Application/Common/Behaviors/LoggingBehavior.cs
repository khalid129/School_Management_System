using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SchoolPortal.Application.Common.Interfaces;

namespace SchoolPortal.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline step: logs each request name, the resolved tenant, and elapsed time.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ITenantContext tenantContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogInformation(
            "Handling {RequestName} (tenant {SchoolId})",
            requestName, tenantContext.CurrentSchoolId);

        try
        {
            var response = await next();
            stopwatch.Stop();
            logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs} ms",
                requestName, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogWarning(
                ex, "{RequestName} failed after {ElapsedMs} ms",
                requestName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
