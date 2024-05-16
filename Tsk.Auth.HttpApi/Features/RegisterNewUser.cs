using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure.FeaturesDiscovery;
using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.Entities;
using Tsk.Auth.HttpApi.Passwords;

namespace Tsk.Auth.HttpApi.Features;

public static class RegisterNewUserFeature
{
    [PublicAPI]
    public sealed class RegisterNewUserDto
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
    public sealed class RegisteredUserDto
    {
        public required Guid Id { get; init; }

        public required string Email { get; init; }
    }

    public sealed class Controller : ApiControllerBase
    {
        private readonly TskAuthDbContext dbContext;
        private readonly IPasswordHandler passwordHandler;

        public Controller(TskAuthDbContext dbContext, IPasswordHandler passwordHandler)
        {
            this.dbContext = dbContext;
            this.passwordHandler = passwordHandler;
        }

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
                return ValidationProblem(
                    property: () => registerNewUserDto.Email,
                    message: "Another account with provided email already exists."
                );
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = registerNewUserDto.Email,
                Password = passwordHandler.HashPassword(registerNewUserDto.Password)
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
