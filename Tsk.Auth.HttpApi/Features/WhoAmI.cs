using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.Auth.HttpApi.Features;

public static class WhoAmIFeature
{
    public sealed class AccessTokenPayloadDto
    {
        public required Guid Id { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        [HttpGet("/who-am-i")]
        [Authorize]
        [ProducesResponseType<AccessTokenPayloadDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult WhoAmI()
        {
            var userIdClaim = User.Claims.Single(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            var userId = Guid.Parse(userIdClaim.Value);

            var accessTokenPayloadDto = new AccessTokenPayloadDto
            {
                Id = userId
            };
            return Ok(accessTokenPayloadDto);
        }
    }
}
