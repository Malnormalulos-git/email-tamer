using EmailTamer.Dependencies.Amazon.Config;
using FluentValidation;

namespace EmailTamer.Dependencies.Amazon.Validation;

internal class BucketNameValidator : AbstractValidator<IHasBucketName>
{
    public BucketNameValidator() => RuleFor(x => x.BucketName).NotNull().NotEmpty().AmazonBucketName();
}