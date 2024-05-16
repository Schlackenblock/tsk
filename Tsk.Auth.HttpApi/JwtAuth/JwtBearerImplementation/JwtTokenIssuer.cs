using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tsk.Auth.HttpApi.JwtAuth.Abstractions;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public sealed class JwtTokenIssuer : IJwtTokenIssuer
{
    private readonly IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot;

    public JwtTokenIssuer(IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot)
    {
        this.jwtAuthOptionsSnapshot = jwtAuthOptionsSnapshot;
    }

    public async Task<TokenPair> IssueJwtTokenPair(Guid bearerId, Guid refreshTokenId)
    {
        var accessToken = await IssueTokenAsync(
            claims: new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, bearerId.ToString() }
            },
            lifetime: TimeSpan.FromMinutes(jwtAuthOptionsSnapshot.Value.AccessTokenLifetimeInMinutes)
        );

        var refreshToken = await IssueTokenAsync(
            claims: new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Jti, refreshTokenId.ToString() }
            },
            lifetime: TimeSpan.FromMinutes(jwtAuthOptionsSnapshot.Value.RefreshTokenLifetimeInMinutes)
        );

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private async Task<string> IssueTokenAsync(IDictionary<string, object> claims, TimeSpan lifetime)
    {
        // JwtBearer doesn't work properly with Guid, but we can manually convert it to string.
        foreach (var claim in claims)
        {
            if (claim.Value is Guid)
            {
                claims[claim.Key] = claim.Value.ToString()!;
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = DateTime.UtcNow.Add(lifetime),
            SigningCredentials = await GetSigningCredentialsAsync()
        };

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(securityToken);
    }

    private async Task<SigningCredentials> GetSigningCredentialsAsync()
    {
        var jwtSigningKeyPath = jwtAuthOptionsSnapshot.Value.SigningKeyPath;
        var jwtSigningKeyXml = await File.ReadAllTextAsync(jwtSigningKeyPath);

        var jwtSigningKey = RSA.Create();
        jwtSigningKey.FromXmlString(jwtSigningKeyXml);

        var rsaSecurityKey = new RsaSecurityKey(jwtSigningKey);
        return new SigningCredentials(rsaSecurityKey, SecurityAlgorithms.RsaSha256);
    }
}
