using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sellorio.AudioOracle.Library;

public static class AsyncEnumerableExtensions
{
    public static async IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> source, int count)
    {
        int takenCount = 0;

        await foreach (var item in source)
        {
            if (takenCount >= count)
            {
                yield break;
            }

            yield return item;
            takenCount++;
        }
    }

    public static async IAsyncEnumerable<T> Skip<T>(this IAsyncEnumerable<T> source, int count)
    {
        int skippedCount = 0;

        await foreach (var item in source)
        {
            if (skippedCount < count)
            {
                skippedCount++;
                continue;
            }

            yield return item;
        }
    }

    public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
    {
        const int DefaultCapacity = 4;
        var buffers = new List<T[]>();
        int totalCount = 0;

        // Start with small buffer and double each time
        var current = new T[DefaultCapacity];
        int index = 0;

        await foreach (var item in source)
        {
            if (index == current.Length)
            {
                buffers.Add(current);
                current = new T[current.Length * 2];
                index = 0;
            }

            current[index++] = item;
            totalCount++;
        }

        // Final buffer
        buffers.Add(current);

        // Flatten into result array
        var result = new T[totalCount];
        int offset = 0;

        for (int i = 0; i < buffers.Count; i++)
        {
            var buffer = buffers[i];
            int count = (i == buffers.Count - 1) ? index : buffer.Length;
            Array.Copy(buffer, 0, result, offset, count);
            offset += count;
        }

        return result;
    }
}
