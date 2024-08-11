using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Tsk.HttpApi.Features.ForAdmins.Auth;

[PublicAPI]
public class TokenPairDto
{
    public required TokenDto AccessToken { get; init; }
    public required TokenDto RefreshToken { get; init; }
}

[PublicAPI]
public class TokenDto
{
    public required string Value { get; init; }
    public required int ExpiresIn { get; init; }
}

[PublicAPI]
public class UserCredentialsDto
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }
}
