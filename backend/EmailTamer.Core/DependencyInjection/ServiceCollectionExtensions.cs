using System.Reflection;
using System.Runtime.CompilerServices;
using EmailTamer.Core.Config;
using EmailTamer.Core.FluentValidation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmailTamer.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IServiceCollection AddCoreServicesFromAssemblyContaining<T>(this IServiceCollection services)
		=> services.AddCoreServicesFromAssembly(typeof(T).Assembly);

	public static IServiceCollection AddCoreServicesFromAssembly(this IServiceCollection services, Assembly assembly)
	{
		services.AddValidators(assembly);
		services.AddAutoMapper(assembly);
		services.AddMediatR(x => x.RegisterServicesFromAssembly(assembly));

		return services;
	}

	private static IServiceCollection AddValidators(this IServiceCollection services, Assembly assembly)
		=> services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true, filter: result =>
		{
			var validatorType = result.ValidatorType;
			if (validatorType.CustomAttributes.Any(x => x.AttributeType == typeof(NotInjectableValidatorAttribute)))
			{
				return false;
			}

			var validatorInterface = result.InterfaceType;
			if (validatorInterface is not { IsInterface: true, IsConstructedGenericType: true })
			{
				return true;
			}

			if (validatorInterface.GetGenericTypeDefinition() != typeof(IValidator<>))
			{
				return true;
			}

			var parameter = validatorInterface.GetGenericArguments()[0];
			return !parameter.IsAssignableTo(typeof(IAppConfig));
		});
}