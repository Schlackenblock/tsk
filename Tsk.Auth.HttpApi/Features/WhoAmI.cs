using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.Client.Sessions;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.FeaturesDiscovery;

namespace Tsk.Auth.HttpApi.Features;

public static class WhoAmIFeature
{
    [PublicAPI]
    public sealed class CurrentUserDto
    {
        public required Guid Id { get; init; }
        public required string Email { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly TskAuthDbContext dbContext;

        public Controller(ICurrentUserAccessor currentUserAccessor, TskAuthDbContext dbContext)
        {
            this.currentUserAccessor = currentUserAccessor;
            this.dbContext = dbContext;
        }

        [HttpGet("/who-am-i")]
        [Authorize]
        [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> WhoAmI()
        {
            var currentUserId = currentUserAccessor.CurrentUser.Id;

            var user = await dbContext.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.Id == currentUserId);
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
