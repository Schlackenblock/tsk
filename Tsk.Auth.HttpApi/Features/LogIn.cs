using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Entities;
using Tsk.Auth.HttpApi.FeaturesDiscovery;
using Tsk.Auth.HttpApi.JwtAuth.Abstractions;
using Tsk.Auth.HttpApi.Passwords;

namespace Tsk.Auth.HttpApi.Features;

public static class LogInFeature
{
    [PublicAPI]
    public sealed class LogInDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(254)]
        public required string Email { get; init; }

        [Required]
        [MaxLength(20)]
        public required string Password { get; init; }
    }

    [PublicAPI]
    public sealed class JwtTokenPairDto
    {
        public required string AccessToken { get; set; }

        public required string RefreshToken { get; set; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly TskAuthDbContext dbContext;
        private readonly IJwtTokenIssuer jwtTokenIssuer;
        private readonly IPasswordHandler passwordHandler;

        public Controller(TskAuthDbContext dbContext, IJwtTokenIssuer jwtTokenIssuer, IPasswordHandler passwordHandler)
        {
            this.dbContext = dbContext;
            this.jwtTokenIssuer = jwtTokenIssuer;
            this.passwordHandler = passwordHandler;
        }

        [HttpPost("/log-in")]
        [ProducesResponseType<JwtTokenPairDto>(StatusCodes.Status200OK)]
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

            var correctPassword = passwordHandler.VerifyPassword(
                plainTextPassword: logInDto.Password,
                hashedPassword: user.Password
            );
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

            var jwtTokenPair = await jwtTokenIssuer.IssueJwtTokenPair(user.Id, newSession.RefreshTokenId);
            var jwtTokenPairDto = new JwtTokenPairDto
            {
                AccessToken = jwtTokenPair.AccessToken,
                RefreshToken = jwtTokenPair.RefreshToken
            };
            return Ok(jwtTokenPairDto);
        }
    }
}
