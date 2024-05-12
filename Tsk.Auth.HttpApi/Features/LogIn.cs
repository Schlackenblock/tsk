using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.Context;

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
    }

    public sealed class Controller(TskAuthContext dbContext) : ApiControllerBase
    {
        [HttpPost("/log-in")]
        public async Task<IActionResult> RegisterNewUser([FromBody] LogInDto logInDto)
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

            var currentUserDto = new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email
            };
            return Ok(currentUserDto);
        }
    }
}
