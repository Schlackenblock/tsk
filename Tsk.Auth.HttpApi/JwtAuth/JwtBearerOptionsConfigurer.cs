using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.HttpApi.JwtAuth;

public class JwtBearerOptionsConfigurer : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IOptions<JwtAuthOptions> jwtAuthOptions;

    public JwtBearerOptionsConfigurer(IOptions<JwtAuthOptions> jwtAuthOptions) =>
        this.jwtAuthOptions = jwtAuthOptions;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = GetVerificationKey(),
            ValidateIssuerSigningKey = true
        };

        options.MapInboundClaims = false;
    }

    private RsaSecurityKey GetVerificationKey()
    {
        var jwtVerificationKeyPath = jwtAuthOptions.Value.VerificationKeyPath;
        var jwtVerificationKeyXml = File.ReadAllText(jwtVerificationKeyPath);

        var jwtVerificationKey = RSA.Create();
        jwtVerificationKey.FromXmlString(jwtVerificationKeyXml);

        return new RsaSecurityKey(jwtVerificationKey);
    }
}
