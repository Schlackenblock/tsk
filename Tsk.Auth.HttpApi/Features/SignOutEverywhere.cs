using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;

namespace Tsk.Auth.HttpApi.Features;

public static class SignOutEverywhereFeature
{
    public sealed class Controller : ApiControllerBase
    {
        private readonly TskAuthContext dbContext;

        public Controller(TskAuthContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("/sign-out-everywhere")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignOutEverywhere()
        {
            var userIdClaim = User.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var userId = Guid.Parse(userIdClaim.Value);

            await dbContext.Sessions
                .Where(session => session.UserId == userId)
                .ExecuteDeleteAsync();

            return Ok();
        }
    }
}
