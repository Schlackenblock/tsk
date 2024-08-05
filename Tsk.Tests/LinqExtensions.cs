namespace Tsk.Tests;

public static class LinqExtensions
{
    public static IReadOnlyCollection<T> Shuffle<T>(this IReadOnlyCollection<T> collection)
    {
        return collection
            .OrderBy(_ => Random.Shared.Next())
            .ToList();
    }

    public static IEnumerable<BackTrackable<T>> WithBackTracking<T>(this IEnumerable<T> enumerable)
        where T : class
    {
        using var enumerator = enumerable.GetEnumerator();

        var isEmpty = enumerator.MoveNext();
        if (!isEmpty)
        {
            yield break;
        }

        var firstElement = enumerator.Current;
        yield return new BackTrackable<T>(Previous: null, Current: firstElement);

        var previousElement = firstElement;
        while (enumerator.MoveNext())
        {
            var currentElement = enumerator.Current;
            yield return new BackTrackable<T>(previousElement, currentElement);
            previousElement = currentElement;
        }
    }
}

public record BackTrackable<T>(T? Previous, T Current)
    where T : class;
