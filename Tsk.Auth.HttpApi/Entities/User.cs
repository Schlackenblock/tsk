using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.Auth.HttpApi.Entities;

public sealed class User
{
    public required Guid Id { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}

public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> userEntityBuilder)
    {
        userEntityBuilder
            .HasIndex(user => user.Email)
            .IsUnique();
    }
}
