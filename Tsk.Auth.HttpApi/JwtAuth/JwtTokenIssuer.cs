using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtTokenIssuer
{
    private readonly IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot;

    public JwtTokenIssuer(IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot) =>
        this.jwtAuthOptionsSnapshot = jwtAuthOptionsSnapshot;

    public async Task<string> IssueAccessTokenAsync(Guid bearerId)
    {
        var claims = new Dictionary<string, object>
        {
            // JwtBearer doesn't work properly with Guid, but we can manually convert it to string.
            { JwtRegisteredClaimNames.Sub, bearerId.ToString() }
        };

        var accessTokenLifetime = TimeSpan.FromMinutes(jwtAuthOptionsSnapshot.Value.AccessTokenLifetimeInMinutes);
        var expirationDateTime = DateTime.UtcNow.Add(accessTokenLifetime);

        var signingCredentials = await GetSigningCredentialsAsync();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            Expires = expirationDateTime,
            SigningCredentials = signingCredentials
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
