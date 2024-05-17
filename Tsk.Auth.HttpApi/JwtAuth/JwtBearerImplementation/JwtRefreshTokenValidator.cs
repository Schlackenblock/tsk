using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tsk.Auth.Client.Options;
using Tsk.Auth.HttpApi.JwtAuth.Abstractions;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public sealed class JwtRefreshTokenValidator : IJwtRefreshTokenValidator
{
    private readonly IOptionsSnapshot<TskAuthOptions> tskAuthOptionsSnapshot;

    public JwtRefreshTokenValidator(IOptionsSnapshot<TskAuthOptions> tskAuthOptionsSnapshot)
    {
        this.tskAuthOptionsSnapshot = tskAuthOptionsSnapshot;
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
        var jwtVerificationKeyPath = tskAuthOptionsSnapshot.Value.JwtTokenVerificationKeyPath;
        var jwtVerificationKeyXml = await File.ReadAllTextAsync(jwtVerificationKeyPath);

        var jwtVerificationKey = RSA.Create();
        jwtVerificationKey.FromXmlString(jwtVerificationKeyXml);

        return new RsaSecurityKey(jwtVerificationKey);
    }
}
