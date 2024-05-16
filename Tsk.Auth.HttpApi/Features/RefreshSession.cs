using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure.FeaturesDiscovery;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.JwtAuth.Abstractions;

namespace Tsk.Auth.HttpApi.Features;

public static class RefreshSessionFeature
{
    [PublicAPI]
    public sealed class RefreshTokenDto
    {
        [Required]
        public required string RefreshToken { get; init; }
    }

    [PublicAPI]
    public sealed class JwtTokenPairDto
    {
        public required string AccessToken { get; init; }

        public required string RefreshToken { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly IJwtRefreshTokenValidator refreshTokenValidator;
        private readonly TskAuthDbContext dbContext;
        private readonly IJwtTokenIssuer jwtTokenIssuer;

        public Controller(
            IJwtRefreshTokenValidator refreshTokenValidator,
            TskAuthDbContext dbContext,
            IJwtTokenIssuer jwtTokenIssuer)
        {
            this.refreshTokenValidator = refreshTokenValidator;
            this.dbContext = dbContext;
            this.jwtTokenIssuer = jwtTokenIssuer;
        }

        [HttpPost("/refresh-session")]
        [ProducesResponseType<JwtTokenPairDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshSession([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var validationResult = await refreshTokenValidator.ValidateRefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (validationResult is not RefreshTokenIsValid successfulValidationResult)
            {
                return RefreshTokenValidationProblem(
                    property: () => refreshTokenDto.RefreshToken,
                    validationResult: validationResult
                );
            }

            var oldRefreshToken = successfulValidationResult.RefreshTokenId;
            var session = await dbContext.Sessions
                .Where(session => session.RefreshTokenId == oldRefreshToken)
                .SingleOrDefaultAsync();
            if (session is null)
            {
                // Provided refresh token is valid and fresh (not expired), but is not presented in the DB.
                // This can only be one of 2 things:
                // 1. We messed up and didn't save RefreshToken's ID when issued it.
                // 2. Attacker obtained our signing key and issued this refresh token.
                // In both cases it would be better to notify someone about this, but, since I'm lazy, I won't.

                return ValidationProblem(
                    property: () => refreshTokenDto.RefreshToken,
                    message: "Invalid refresh token provided."
                );
            }

            var newRefreshTokenId = Guid.NewGuid();
            session.RefreshTokenId = newRefreshTokenId;
            await dbContext.SaveChangesAsync();

            var newJwtTokenPair = await jwtTokenIssuer.IssueJwtTokenPair(session.UserId, newRefreshTokenId);
            var newJwtTokenPairDto = new JwtTokenPairDto
            {
                AccessToken = newJwtTokenPair.AccessToken,
                RefreshToken = newJwtTokenPair.RefreshToken
            };
            return Ok(newJwtTokenPairDto);
        }

        private IActionResult RefreshTokenValidationProblem(
            Expression<Func<object>> property,
            IRefreshTokenValidationResult validationResult)
        {
            return validationResult switch
            {
                RefreshTokenInvalid => ValidationProblem(
                    property: property,
                    message: "Invalid refresh token provided."
                ),
                RefreshTokenExpired => ValidationProblem(
                    property: property,
                    message: "Expired refresh token provided."
                ),
                _ => throw new UnreachableException()
            };
        }
    }
}
