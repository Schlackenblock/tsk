using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

    public sealed class Controller(TskAuthContext dbContext, IOptionsSnapshot<JwtAuthOptions> jwtAuthOptions) : ApiControllerBase
    {
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

            var accessToken = IssueAccessToken(user.Id);

            var currentUserDto = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                AccessToken = accessToken
            };
            return Ok(currentUserDto);
        }

        private string IssueAccessToken(Guid bearerId)
        {
            var claims = new Dictionary<string, object>
            {
                { JwtRegisteredClaimNames.Sub, bearerId.ToString() }
            };

            var accessTokenLifetime = TimeSpan.FromMinutes(jwtAuthOptions.Value.AccessTokenLifetimeInMinutes);
            var expirationDateTime = DateTime.UtcNow.Add(accessTokenLifetime);

            var signingCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Value.SigningKey)),
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = expirationDateTime,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
