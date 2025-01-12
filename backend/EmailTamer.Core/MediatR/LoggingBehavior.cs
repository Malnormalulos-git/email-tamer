using EmailTamer.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmailTamer.Core.MediatR;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
	: IPipelineBehavior<TRequest, TResponse>
	where TRequest : IBaseRequest
{
	public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
								  CancellationToken cancellationToken)
		=> logger.TimeAsync(new Func<Task<TResponse>>(next), typeof(TRequest).Name);
}