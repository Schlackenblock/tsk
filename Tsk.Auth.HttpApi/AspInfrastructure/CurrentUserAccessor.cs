using System.IdentityModel.Tokens.Jwt;

namespace Tsk.Auth.HttpApi.AspInfrastructure;

public sealed class CurrentUserAccessor
{
    public CurrentUser CurrentUser => lazyCurrentUser.Value;
    private readonly Lazy<CurrentUser> lazyCurrentUser;

    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.lazyCurrentUser = new Lazy<CurrentUser>(GetCurrentUser);
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

public sealed class CurrentUser
{
    public required Guid Id { get; init; }
}

public static class CurrentUserAccessorInjection
{
    public static void AddCurrentUserAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<CurrentUserAccessor>();
    }
}
