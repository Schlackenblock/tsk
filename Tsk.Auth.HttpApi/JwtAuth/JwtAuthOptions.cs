using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtAuthOptions
{
    [Required]
    [MinLength(32)]
    [UsedImplicitly]
    public required string SigningKey { get; init; }

    [Required]
    [Range(1, 15)]
    [UsedImplicitly]
    public int AccessTokenLifetimeInMinutes { get; init; }
}
