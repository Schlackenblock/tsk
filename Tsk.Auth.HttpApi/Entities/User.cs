namespace Tsk.Auth.HttpApi.Entities;

public sealed class User
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string Password { get; init; }
}
