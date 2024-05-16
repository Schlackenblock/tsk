using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public sealed class JwtAuthOptions
{
    [Required]
    [UsedImplicitly]
    public required string SigningKeyPath { get; init; }

    [Required]
    [UsedImplicitly]
    public required string VerificationKeyPath { get; init; }

    [Required]
    [Range(1, 15)]
    [UsedImplicitly]
    public int AccessTokenLifetimeInMinutes { get; init; }

    [Required]
    [Range(1, 90 * 24 * 60 /* up to 90 days */)]
    [UsedImplicitly]
    public int RefreshTokenLifetimeInMinutes { get; init; }
}
