using EmailTamer.Core.Models;

namespace EmailTamer.Auth.Models;

public sealed class AuthResponseDto : IOutbound
{
    public string Token { get; init; } = null!;
}