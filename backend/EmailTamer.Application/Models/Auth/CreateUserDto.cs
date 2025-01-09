using EmailTamer.Auth;
using FluentValidation;

namespace EmailTamer.Models.Auth;

public class CreateUserDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
    
    public UserRole Role { get; set; } = UserRole.User;
    
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .NotNull()
                .Email();
            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .Password();
            RuleFor(x => x.Role)
                .IsInEnum()
                .NotEqual(UserRole.Admin);
        }
    }
}