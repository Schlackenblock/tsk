using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtRefreshTokenValidator
{
    private readonly IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot;

    public JwtRefreshTokenValidator(IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot)
    {
        this.jwtAuthOptionsSnapshot = jwtAuthOptionsSnapshot;
    }

    public async Task<IRefreshTokenValidationResult> ValidateRefreshTokenAsync(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = await GetVerificationCredentialsAsync(),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out _);
            var refreshTokenIdClaim = claimsPrincipal.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Jti);

            var refreshTokenId = Guid.Parse(refreshTokenIdClaim.Value);
            return new RefreshTokenIsValid
            {
                RefreshTokenId = refreshTokenId
            };
        }
        catch (SecurityTokenExpiredException)
        {
            return new RefreshTokenExpired();
        }
        catch
        {
            return new RefreshTokenInvalid();
        }
    }

    private async Task<SecurityKey> GetVerificationCredentialsAsync()
    {
        var jwtVerificationKeyPath = jwtAuthOptionsSnapshot.Value.VerificationKeyPath;
        var jwtVerificationKeyXml = await File.ReadAllTextAsync(jwtVerificationKeyPath);

        var jwtVerificationKey = RSA.Create();
        jwtVerificationKey.FromXmlString(jwtVerificationKeyXml);

        return new RsaSecurityKey(jwtVerificationKey);
    }
}

public interface IRefreshTokenValidationResult;

public sealed class RefreshTokenIsValid : IRefreshTokenValidationResult
{
    public required Guid RefreshTokenId { get; init; }
}

public sealed class RefreshTokenInvalid : IRefreshTokenValidationResult;

public sealed class RefreshTokenExpired : IRefreshTokenValidationResult;
