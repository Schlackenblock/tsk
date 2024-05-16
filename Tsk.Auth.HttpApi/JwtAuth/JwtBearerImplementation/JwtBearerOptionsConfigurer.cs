using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;

public class JwtBearerOptionsConfigurer : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IOptionsMonitor<JwtAuthOptions> jwtAuthOptionsMonitor;

    public JwtBearerOptionsConfigurer(IOptionsMonitor<JwtAuthOptions> jwtAuthOptionsMonitor)
    {
        this.jwtAuthOptionsMonitor = jwtAuthOptionsMonitor;
    }

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKeyResolver = ResolveVerificationKey,
            ValidateIssuerSigningKey = true
        };

        options.MapInboundClaims = false;
    }

    private IEnumerable<SecurityKey> ResolveVerificationKey(
        string _,
        SecurityToken __,
        string ___,
        TokenValidationParameters ____)
    {
        var jwtVerificationKeyPath = jwtAuthOptionsMonitor.CurrentValue.VerificationKeyPath;
        var jwtVerificationKeyXml = File.ReadAllText(jwtVerificationKeyPath);

        var jwtVerificationKey = RSA.Create();
        jwtVerificationKey.FromXmlString(jwtVerificationKeyXml);

        var verificationKey = new RsaSecurityKey(jwtVerificationKey);
        return [verificationKey];
    }
}
