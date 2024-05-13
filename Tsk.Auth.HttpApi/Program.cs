using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.JwtAuth;
using Tsk.Auth.HttpApi.Swagger;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddDbContext<TskAuthContext>(options =>
{
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString("PostgreSQL");
    options.UseNpgsql(connectionString);
    options.UseSnakeCaseNamingConvention();
});

webApplicationBuilder.Services
    .AddOptions<JwtAuthOptions>()
    .BindConfiguration(nameof(JwtAuthOptions))
    .ValidateDataAnnotations()
    .ValidateOnStart();

webApplicationBuilder.Services.AddScoped<JwtTokenIssuer>();

webApplicationBuilder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var verificationKey = RSA.Create();
        verificationKey.FromXmlString("<RSAKeyValue><Modulus>yIGNNMFPyV/FSS/KKKRlrigZddvIdFrz/42yYpXNM2E8iYhzbTZLuNXkpDe1r5uA8WJgdupp9QyKc0fa1JCt+sLhy7H5g9zohgc7RLyD7cK89gKfikLUXpgywmaMbZ0EiZNUSCEcPRO6G6ikekCe1ryPf1FxhL+36nHYBgT52ZzurHKlblZ1uIvh6zpxx8sFSo9EIsBaEXVjI49CqSZM0LULCO4NI6kDPL2snk8ucmAYSosPtmzGIPpYrde46xdsCNsERTSjgNT2moivA9EsA9wsGnZunVEd0ubWhoazb+bF+iys4XeMpesPW3rPpppMNjYi31NTfw/9TZv0LMxsWQ==</Modulus><Exponent>AQAB</Exponent><P>69eHvWS2sfgq5lUAR9MWDciCQpm8q4nSh0AavQgwpeaYtUsmk+yFsj2roNHinbQiQg7YIwuiRexFjDwY5RzPpqKdNxNrf2sAuxvGu6LEox8WAlK6RbydAeDn8r7dBZOxEVK8Vjaoyzx6iGp99v91TdpRhU3pn/x/AqhdlfABq28=</P><Q>2aTV6mJpjVQQca8jqVHxXfGuEdVY5LMgfOgfAX14yva3FOHRtEGGSyhrC9D+qKTVUCJtF3Hum77NKAd8gmV3bEzURgUOtxTUOBlIeBCyxJpakk4413TGphqMS0E0KChW8nsp+j2jUxqaaPuxBWq1ziFXrBuaUHThjXfW4WojILc=</Q><DP>X1+25hF/jHMmriT7yxADICnQ+6v+T4SZ3dY/uehA/CZH5TDx9eo1mwIzkQKv2UTKgdpM78c5pXtKGM8I5kaDdwjV+TriQxH7pq8x7BmD2c+tb90StTb0a5kPU+x5p1K1zaKexWKvWhkoGFiwfh2Ky6QZSYZbjzZmCqYZypPXqQM=</DP><DQ>UcqNjaDgU/l0mJSeDUK7N3fc19zapE/g9K/y/wddPsFK+nEG8vvw8QvGdtFa8pvHgm79mmL5rdQdm1583zAsmimhWJML/ae5PT/bKSv1UsdDJFp7KBU9lizB7oIZjMHkMwowM1e4qkGEJ9H2q7d3jXigTxCshHH4VcbknR/bBa8=</DQ><InverseQ>q+5nTfHL41Xq2hjUVI4g/uIPdfYp3lgDhQqJu5ZDRSARsvnM3wY7MyYSkowaMIWDb54uVNZ3CazIMMHYWRuH/bpab7jb53G66qPnIuJmgFQg8F6RmlIHKrupued29fOz7ii/yGtoiRq1Ma46FO3Y+sXjwXfzkfEg0HCPr8SE7QU=</InverseQ><D>EsN06GvywICS4M91FqqTzWF5SHtB3gmo7dBf0jP8mQNPe/tl+eFr4qc5l1iTSxS9U1mGd3pmYgVDa5hc1SmY7m+QW9SS6hEhPXGtzTfrDLP9pPQH5BNu9k896M1Z9OQNyaYc7AcVMm4HK5FzmvEzLBtHPn3rpqWeW3U95wpOK1wG1bYRKrW7EBXVYefML+8oDsejnOetrZJpuxF8pSmJF9Tw5eSxAt8n7beBGxFk+AO9EN5lHsYNMi90RnFgVvOwct1aRghILSDIMJXWwWU1JlW9PiTY2MXHEltqBKM42OVOvH/1JE49K2mEMjFfWjd5hvEro3+94Ej7aZVWrAv9xQ==</D></RSAKeyValue>");

        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new RsaSecurityKey(verificationKey),
            ValidateIssuerSigningKey = true
        };

        options.MapInboundClaims = false;
    });

webApplicationBuilder.Services.AddAuthorization();

webApplicationBuilder
    .Services
    .AddControllers()
    .ConfigureControllerDiscoverer();

webApplicationBuilder.Services.AddSwaggerGen(options =>
{
    options.UseUniqueSchemaIds();
    options.AddJwtAuthentication();
});

var webApplication = webApplicationBuilder.Build();

webApplication.UseSwagger(Environments.Development);
webApplication.UseRouting();

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapControllers();

webApplication.Run();
