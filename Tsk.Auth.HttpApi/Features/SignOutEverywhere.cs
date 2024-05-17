using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.Client.Sessions;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.FeaturesDiscovery;

namespace Tsk.Auth.HttpApi.Features;

public static class SignOutEverywhereFeature
{
    public sealed class Controller : ApiControllerBase
    {
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly TskAuthDbContext dbContext;

        public Controller(ICurrentUserAccessor currentUserAccessor, TskAuthDbContext dbContext)
        {
            this.currentUserAccessor = currentUserAccessor;
            this.dbContext = dbContext;
        }

        [HttpPost("/sign-out-everywhere")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignOutEverywhere()
        {
            var currentUserId = currentUserAccessor.CurrentUser.Id;

            await dbContext.Sessions
                .Where(session => session.UserId == currentUserId)
                .ExecuteDeleteAsync();

            return Ok();
        }
    }
}
