namespace Tsk.Tests;

public static class LinqExtensions
{
    public static IReadOnlyCollection<T> Shuffle<T>(this IReadOnlyCollection<T> list)
    {
        return list
            .OrderBy(_ => Random.Shared.Next())
            .ToList();
    }
}
