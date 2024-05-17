using JetBrains.Annotations;

namespace Tsk.Auth.Client.Sessions;

[PublicAPI]
public interface ICurrentUserAccessor
{
    CurrentUser CurrentUser { get; }
}

[PublicAPI]
public sealed class CurrentUser
{
    public required Guid Id { get; init; }
}
