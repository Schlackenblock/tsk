namespace Tsk.Tests;

public static class LinqExtensions
{
    public static IReadOnlyCollection<T> Shuffle<T>(this IReadOnlyCollection<T> collection)
    {
        return collection
            .OrderBy(_ => Random.Shared.Next())
            .ToList();
    }
}
