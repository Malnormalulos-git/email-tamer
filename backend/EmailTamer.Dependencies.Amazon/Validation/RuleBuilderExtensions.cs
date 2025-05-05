using System.Text.RegularExpressions;
using FluentValidation;

namespace EmailTamer.Dependencies.Amazon.Validation;

internal static partial class RuleBuilderExtensions
{
    private static readonly Regex amazonBucketNameRegex = GenerateAmazonBucketNameRegex();
    public static IRuleBuilderOptions<T, string?> AmazonBucketName<T>(this IRuleBuilder<T, string?> builder)
        => builder.Matches(amazonBucketNameRegex);

    [GeneratedRegex("(?!(^((2(5[0-5]|[0-4][0-9])|[01]?[0-9]{1,2})\\.){3}(2(5[0-5]|[0-4][0-9])|[01]?[0-9]{1,2})$|^xn--|.+-s3alias$))^[a-z0-9][a-z0-9.-]{1,61}[a-z0-9]$", RegexOptions.Compiled)]
    private static partial Regex GenerateAmazonBucketNameRegex();
}