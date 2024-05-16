using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure.FeaturesDiscovery;
using Tsk.Auth.HttpApi.AspInfrastructure.Sessions;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Passwords;

namespace Tsk.Auth.HttpApi.Features;

public static class ChangePasswordFeature
{
    [PublicAPI]
    public sealed class ChangePasswordDto
    {
        [Required]
        [MaxLength(20)]
        public required string OldPassword { get; init; }

        [Required]
        [MaxLength(20)]
        public required string NewPassword { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly ICurrentUserAccessor currentUserAccessor;
        private readonly TskAuthDbContext dbContext;
        private readonly IPasswordHandler passwordHandler;

        public Controller(
            ICurrentUserAccessor currentUserAccessor,
            TskAuthDbContext dbContext,
            IPasswordHandler passwordHandler)
        {
            this.currentUserAccessor = currentUserAccessor;
            this.dbContext = dbContext;
            this.passwordHandler = passwordHandler;
        }

        [HttpPost("/change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var currentUserId = currentUserAccessor.CurrentUser.Id;
            var currentUser = await dbContext.Users.SingleOrDefaultAsync(user => user.Id == currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }

            var providedOldPasswordIsCorrect = passwordHandler.VerifyPassword(
                plainTextPassword: changePasswordDto.OldPassword,
                hashedPassword: currentUser.Password
            );
            if (!providedOldPasswordIsCorrect)
            {
                return ValidationProblem(
                    property: () => changePasswordDto.OldPassword,
                    message: "Incorrect password provided."
                );
            }

            await dbContext.Sessions
                .Where(session => session.UserId == currentUserId)
                .ExecuteDeleteAsync();

            var hashedNewPassword = passwordHandler.HashPassword(changePasswordDto.NewPassword);
            currentUser.Password = hashedNewPassword;
            await dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
