using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tsk.Auth.Client.Sessions;

internal sealed class CurrentUserAccessor : ICurrentUserAccessor
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

internal static class CurrentUserAccessorDependencyInjection
{
    public static void AddCurrentUserAccessor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
    }
}
