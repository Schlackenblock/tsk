namespace Tsk.Auth.HttpApi.JwtAuth.Abstractions;

public interface IJwtRefreshTokenValidator
{
    Task<IRefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken);
}

public interface IRefreshTokenValidationResult;

public sealed class RefreshTokenIsValid : IRefreshTokenValidationResult
{
    public required Guid RefreshTokenId { get; init; }
}

public sealed class RefreshTokenInvalid : IRefreshTokenValidationResult;

public sealed class RefreshTokenExpired : IRefreshTokenValidationResult;
