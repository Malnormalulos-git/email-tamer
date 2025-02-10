using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using EmailTamer.Core.Persistence;
using EmailTamer.Dependencies.Amazon.Config;
using EmailTamer.Dependencies.Amazon.Persistence;
using EmailTamer.Dependencies.Amazon.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace EmailTamer.Dependencies.Amazon;

public static class ServiceCollectionExtensions
{
	private static readonly RegionEndpoint region = RegionEndpoint.EUWest1;

	public static IServiceCollection AddAmazonServices(this IServiceCollection services, bool isDevelopment)
	{
		var awsOptions = new AWSOptions
		{
			Region = region
		};

		services.AddDefaultAWSOptions(awsOptions);
		services.AddAmazonS3(isDevelopment);
		services.TryAddSingleton<IValidator<IHasBucketName>, BucketNameValidator>();

		return services;
	}

	private static IServiceCollection AddAmazonS3(this IServiceCollection services, bool isDevelopment)
	{
		var s3Config = isDevelopment
			? new AmazonS3Config
			{
				ServiceURL = "http://localhost:4566",
				ForcePathStyle = true,
				UseHttp = true
			}
			: new AmazonS3Config
			{
				RegionEndpoint = region
			};

		services.AddSingleton<IAmazonS3>(new AmazonS3Client(s3Config));
		services.TryAddTransient<IBlobStorage, S3BlobStorage>();

		return services;
	}
}