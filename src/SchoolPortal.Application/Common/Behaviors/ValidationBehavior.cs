using FluentValidation;
using MediatR;
using ValidationException = SchoolPortal.Application.Common.Exceptions.ValidationException;

namespace SchoolPortal.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline step: runs every registered FluentValidation validator for the
/// request before the handler. Throws <see cref="ValidationException"/> on the first
/// batch of failures.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f is not null).ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}
