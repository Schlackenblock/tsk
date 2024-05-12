using Microsoft.AspNetCore.Mvc;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Entities;

namespace Tsk.Auth.HttpApi.Features;

public static class RegisterNewUserFeature
{
    public sealed class RegisterNewUserDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

    public sealed class RegisteredUserDto
    {
        public required Guid Id { get; init; }
        public required string Email { get; init; }
    }

    public sealed class Controller(TskAuthContext dbContext) : ApiControllerBase
    {
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterNewUserDto registerNewUserDto)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = registerNewUserDto.Email,
                Password = registerNewUserDto.Password
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            var registeredUserDto = new RegisteredUserDto
            {
                Id = newUser.Id,
                Email = newUser.Email
            };
            return Ok(registeredUserDto);
        }
    }
}
