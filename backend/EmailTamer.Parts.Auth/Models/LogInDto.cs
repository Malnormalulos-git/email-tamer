using FluentValidation;

namespace EmailTamer.Auth.Models;

public sealed class LogInDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public class LogInDtoValidator : AbstractValidator<LogInDto>
    {
        public LogInDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}