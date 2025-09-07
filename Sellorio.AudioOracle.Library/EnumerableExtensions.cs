using System.Collections.Generic;

namespace Sellorio.AudioOracle.Library;

public static class EnumerableExtensions
{
    public static IEnumerable<TValue> TakeEvery<TValue>(this IEnumerable<TValue> enumerable, int n, int offset = 0)
    {
        int index = 0;

        foreach (var item in enumerable)
        {
            if (IsEvery(index, n, offset))
            {
                yield return item;
            }

            index++;
        }
    }

    public static IEnumerable<TValue> SkipEvery<TValue>(this IEnumerable<TValue> enumerable, int n, int offset = 0)
    {
        int index = 0;

        foreach (var item in enumerable)
        {
            if (!IsEvery(index, n, offset))
            {
                yield return item;
            }

            index++;
        }
    }

    private static bool IsEvery(int index, int n, int offset)
    {
        return (index + 1 + offset) % n == 0;
    }
}
