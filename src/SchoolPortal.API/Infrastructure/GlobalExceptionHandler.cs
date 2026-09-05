using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Application.Common.Exceptions;
using SchoolPortal.Domain.Common;

namespace SchoolPortal.API.Infrastructure;

/// <summary>
/// Translates known application/domain exceptions into RFC 7807 Problem Details responses.
/// Registered via <c>AddExceptionHandler</c> + <c>UseExceptionHandler</c>.
/// </summary>
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, errors) = exception switch
        {
            ValidationException ve =>
                (StatusCodes.Status400BadRequest, "One or more validation errors occurred.", ve.Errors),
            DomainException de =>
                (StatusCodes.Status400BadRequest, de.Message, (IReadOnlyDictionary<string, string[]>?)null),
            NotFoundException nf =>
                (StatusCodes.Status404NotFound, nf.Message, null),
            InvalidOperationException io when io.Message.Contains("tenant", StringComparison.OrdinalIgnoreCase) =>
                (StatusCodes.Status409Conflict, io.Message, null),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null),
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Type = $"https://httpstatuses.io/{status}",
        };
        if (errors is not null)
            problemDetails.Extensions["errors"] = errors;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }
}
