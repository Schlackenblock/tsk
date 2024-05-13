using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.JwtAuth;

namespace Tsk.Auth.HttpApi.Features;

public static class LogInFeature
{
    public sealed class LogInDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public sealed class CurrentUserDto
    {
        public required Guid Id { get; init; }
        public required string Email { get; set; }
        public required string AccessToken { get; set; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly TskAuthContext dbContext;
        private readonly JwtTokenIssuer jwtTokenIssuer;

        public Controller(TskAuthContext dbContext, JwtTokenIssuer jwtTokenIssuer)
        {
            this.dbContext = dbContext;
            this.jwtTokenIssuer = jwtTokenIssuer;
        }

        [HttpPost("/log-in")]
        [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogIn([FromBody] LogInDto logInDto)
        {
            var user = await dbContext
                .Users
                .Where(user => user.Email == logInDto.Email)
                .SingleOrDefaultAsync();
            if (user is null)
            {
                return NotFound();
            }

            var correctPassword = BCrypt.Net.BCrypt.EnhancedVerify(logInDto.Password, user.Password);
            if (!correctPassword)
            {
                ModelState.AddModelError(nameof(logInDto.Password), "Incorrect password provided.");
                return ValidationProblem();
            }

            var accessToken = jwtTokenIssuer.IssueAccessToken(user.Id);

            var currentUserDto = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                AccessToken = accessToken
            };
            return Ok(currentUserDto);
        }
    }
}
