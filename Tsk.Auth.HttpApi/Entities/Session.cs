using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tsk.Auth.HttpApi.Entities;

public sealed class Session
{
    public required Guid Id { get; init; }

    public required Guid RefreshTokenId { get; set; }

    public required Guid UserId { get; init; }
}

public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> refreshTokenEntityBuilder)
    {
        refreshTokenEntityBuilder.HasIndex(session => session.RefreshTokenId);

        refreshTokenEntityBuilder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(session => session.UserId);
    }
}
