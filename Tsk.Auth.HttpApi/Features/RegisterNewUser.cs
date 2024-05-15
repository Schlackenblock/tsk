using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Entities;

namespace Tsk.Auth.HttpApi.Features;

public static class RegisterNewUserFeature
{
    public sealed class RegisterNewUserDto
    {
        [Required]
        public required string Email { get; init; }

        [Required]
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
        [ProducesResponseType<RegisteredUserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterNewUserDto registerNewUserDto)
        {
            var otherAccountsUseThisEmail = await dbContext.Users
                .Where(user => user.Email == registerNewUserDto.Email)
                .AnyAsync();
            if (otherAccountsUseThisEmail)
            {
                var errorMessage = "Another account with provided email already exists.";
                ModelState.AddModelError(nameof(registerNewUserDto.Email), errorMessage);
                return ValidationProblem();
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = registerNewUserDto.Email,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(registerNewUserDto.Password)
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
