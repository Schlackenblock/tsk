namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtAuthOptions
{
    public string SigningKey { get; set; }
    public int AccessTokenLifetimeInMinutes { get; set; }
}
