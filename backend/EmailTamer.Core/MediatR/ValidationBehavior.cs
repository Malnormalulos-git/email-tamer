using EmailTamer.Core.Exceptions;
using EmailTamer.Core.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Core.MediatR;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await logger.TimeAsync(async () =>
        {
            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(request, cancellationToken)));
            var failures = validationResults
                .Where(x => !x.IsValid)
                .ToList();

            if (failures.Count != 0)
            {
                var errors = failures.SelectMany(x => x.Errors).ToArray();
                logger.LogWarning("Validation failed for {Request}, failures: {@Failures}", request,
                    errors.Select(x => new { x.ErrorMessage, x.PropertyName, x.ErrorCode }));
                throw new EmailTamerValidationException(errors.Select(x => x.ErrorMessage));
            }
        }, "MediatR request validation");

        return await next();
    }
}