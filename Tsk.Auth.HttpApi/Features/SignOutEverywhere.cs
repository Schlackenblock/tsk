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
        private readonly CurrentUserAccessor currentUserAccessor;
        private readonly TskAuthContext dbContext;

        public Controller(CurrentUserAccessor currentUserAccessor, TskAuthContext dbContext)
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
