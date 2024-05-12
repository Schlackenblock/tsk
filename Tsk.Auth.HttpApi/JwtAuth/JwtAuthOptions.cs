using System.ComponentModel.DataAnnotations;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtAuthOptions
{
    [Required]
    [MinLength(32)]
    public required string SigningKey { get; set; }

    [Required]
    [Range(1, 15)]
    public int AccessTokenLifetimeInMinutes { get; set; }
}
