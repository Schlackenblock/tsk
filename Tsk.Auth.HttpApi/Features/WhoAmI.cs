using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;

namespace Tsk.Auth.HttpApi.Features;

public static class WhoAmIFeature
{
    public sealed class CurrentUserDto
    {
        public required Guid Id { get; init; }
        public required string Email { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly TskAuthContext dbContext;

        public Controller(TskAuthContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("/who-am-i")]
        [Authorize]
        [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> WhoAmI()
        {
            var userIdClaim = User.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var userId = Guid.Parse(userIdClaim.Value);

            var user = await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Id == userId);
            if (user is null)
            {
                return Unauthorized();
            }

            var currentUserDto = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email
            };
            return Ok(currentUserDto);
        }
    }
}
