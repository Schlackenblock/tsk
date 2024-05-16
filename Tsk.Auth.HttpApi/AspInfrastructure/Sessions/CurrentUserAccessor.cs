using System.IdentityModel.Tokens.Jwt;

namespace Tsk.Auth.HttpApi.AspInfrastructure.Sessions;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    public CurrentUser CurrentUser => lazyCurrentUser.Value;
    private readonly Lazy<CurrentUser> lazyCurrentUser;

    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        lazyCurrentUser = new Lazy<CurrentUser>(GetCurrentUser);
    }

    private CurrentUser GetCurrentUser()
    {
        var httpContext = httpContextAccessor.HttpContext;
        var accessTokenClaims = httpContext!.User.Claims;

        var userIdClaim = accessTokenClaims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub);
        var userId = Guid.Parse(userIdClaim.Value);

        return new CurrentUser
        {
            Id = userId
        };
    }
}

public static class CurrentUserAccessorDependencyInjection
{
    public static void AddCurrentUserAccessor(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddHttpContextAccessor();
        webApplicationBuilder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
    }
}
