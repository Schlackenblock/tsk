using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Tsk.Auth.Client.Options;

[PublicAPI]
public sealed class TskAuthOptions
{
    [Required]
    [UsedImplicitly]
    public required string JwtTokenVerificationKeyPath { get; init; }
}
