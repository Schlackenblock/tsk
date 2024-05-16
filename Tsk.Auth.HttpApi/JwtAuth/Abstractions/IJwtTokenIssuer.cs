namespace Tsk.Auth.HttpApi.JwtAuth.Abstractions;

public interface IJwtTokenIssuer
{
    Task<TokenPair> IssueJwtTokenPair(Guid bearerId, Guid refreshTokenId);
}

public sealed class TokenPair
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
