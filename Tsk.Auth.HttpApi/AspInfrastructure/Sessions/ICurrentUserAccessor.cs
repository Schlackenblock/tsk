namespace Tsk.Auth.HttpApi.AspInfrastructure.Sessions;

public interface ICurrentUserAccessor
{
    CurrentUser CurrentUser { get; }
}

public sealed class CurrentUser
{
    public required Guid Id { get; init; }
}
