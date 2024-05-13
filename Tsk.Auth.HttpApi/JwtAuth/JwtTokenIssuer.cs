using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.HttpApi.JwtAuth;

public sealed class JwtTokenIssuer
{
    private readonly IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot;

    public JwtTokenIssuer(IOptionsSnapshot<JwtAuthOptions> jwtAuthOptionsSnapshot) =>
        this.jwtAuthOptionsSnapshot = jwtAuthOptionsSnapshot;

    public string IssueAccessToken(Guid bearerId)
    {
        var claims = new Dictionary<string, object>
        {
            // JwtBearer doesn't work properly with Guid, but we can manually convert it to string.
            { JwtRegisteredClaimNames.Sub, bearerId.ToString() }
        };

        var accessTokenLifetime = TimeSpan.FromMinutes(jwtAuthOptionsSnapshot.Value.AccessTokenLifetimeInMinutes);
        var expirationDateTime = DateTime.UtcNow.Add(accessTokenLifetime);

        var signingCredentials = new SigningCredentials(
            key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptionsSnapshot.Value.SigningKey)),
            // TODO: use asymmetric keys.
            algorithm: SecurityAlgorithms.HmacSha256Signature
        );

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
}
