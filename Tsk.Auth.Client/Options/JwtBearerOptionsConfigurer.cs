using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Tsk.Auth.Client.Options;

internal sealed class JwtBearerOptionsConfigurer : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IOptionsMonitor<TskAuthOptions> tskAuthOptionsMonitor;

    public JwtBearerOptionsConfigurer(IOptionsMonitor<TskAuthOptions> tskAuthOptionsMonitor)
    {
        this.tskAuthOptionsMonitor = tskAuthOptionsMonitor;
    }

    public void PostConfigure(string? _, JwtBearerOptions options)
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
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
        var jwtVerificationKeyPath = tskAuthOptionsMonitor.CurrentValue.JwtTokenVerificationKeyPath;
        var jwtVerificationKeyXml = File.ReadAllText(jwtVerificationKeyPath);

        var jwtVerificationKey = RSA.Create();
        jwtVerificationKey.FromXmlString(jwtVerificationKeyXml);

        var verificationKey = new RsaSecurityKey(jwtVerificationKey);
        return [verificationKey];
    }
}
