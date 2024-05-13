using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtAuthOptions
{
    [Required]
    [UsedImplicitly]
    public required string SigningKeyPath { get; init; }

    [Required]
    [Range(1, 15)]
    [UsedImplicitly]
    public int AccessTokenLifetimeInMinutes { get; init; }
}
