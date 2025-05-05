namespace EmailTamer.Infrastructure.Auth;

public sealed class AuthUser
{
    public string Id { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; }
}