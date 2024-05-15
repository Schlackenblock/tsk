using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Entities;
using Tsk.Auth.HttpApi.JwtAuth;

namespace Tsk.Auth.HttpApi.Features;

public static class LogInFeature
{
    public sealed class LogInDto
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }

    public sealed class TokenPairDto
    {
        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }
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
        [ProducesResponseType<TokenPairDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LogIn([FromBody] LogInDto logInDto)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(user => user.Email == logInDto.Email)
                .SingleOrDefaultAsync();
            if (user is null)
            {
                return ValidationProblem(
                    property: () => logInDto.Email,
                    message: "User with this email doesn't exist."
                );
            }

            var correctPassword = BCrypt.Net.BCrypt.EnhancedVerify(logInDto.Password, user.Password);
            if (!correctPassword)
            {
                return ValidationProblem(
                    property: () => logInDto.Password,
                    message: "Incorrect password provided."
                );
            }

            var newSession = new Session
            {
                Id = Guid.NewGuid(),
                RefreshTokenId = Guid.NewGuid(),
                UserId = user.Id
            };
            dbContext.Sessions.Add(newSession);
            await dbContext.SaveChangesAsync();

            var accessToken = await jwtTokenIssuer.IssueAccessTokenAsync(user.Id);
            var refreshToken = await jwtTokenIssuer.IssueRefreshTokenAsync(newSession.RefreshTokenId);

            var currentUserDto = new TokenPairDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return Ok(currentUserDto);
        }
    }
}
