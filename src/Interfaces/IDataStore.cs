using System.Numerics;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage;

/// <summary>
/// Allows data to be fetched, queried, removed, and stored.
/// </summary>
public interface IDataStore
{
    /// <summary>
    /// Sets the default period after which cached items are considered stale.
    /// </summary>
    TimeSpan DefaultCacheTimeout { get; set; }

    /// <summary>
    /// Indicates whether this <see cref="IDataStore{TItem}"/> implementation allows items to
    /// be cached.
    /// </summary>
    /// <remarks>
    /// This will usually be <see langword="true"/> for implementations which wrap persistent or
    /// networked storage systems, and <see langword="false"/> for in-memory implementations (which
    /// would not typically benefit from caching).
    /// </remarks>
    bool SupportsCaching { get; }

    /// <summary>
    /// Asynchronously computes the average of an <see cref="IDataStoreQueryable{T}"/> evaluated against this
    /// source.
    /// </summary>
    /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IDataStoreQueryable{T}"/> source.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The average of the values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<T> AverageAsync<T>(IDataStoreQueryable<T> source, CancellationToken cancellationToken = default) where T : INumberBase<T>
    {
        try
        {
            var sum = T.Zero;
            var count = 0L;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                sum += item;
                count++;
            }
            return count == 0
                ? throw new InvalidOperationException("sequence contains no elements")
                : sum / T.CreateChecked(count);
        }
        catch (OverflowException) { }

        // overflow occurred, try less precise method which does not use a total
        try
        {
            var average = T.Zero;
            var count = await source.LongCountAsync(cancellationToken);
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = T.CreateChecked(count);

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                average += item / tCount;
            }

            return average;
        }
        catch (OverflowException)
        {
            throw new OverflowException("The average of the sequence is too large to be represented by the type.");
        }
    }

    /// <summary>
    /// Asynchronously computes the sum of an <see cref="IDataStoreQueryable{T}"/> evaluated against this
    /// source.
    /// </summary>
    /// <typeparam name="T">The type of items in <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IDataStoreQueryable{T}"/> source.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The sum of the values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<T> SumAsync<T>(IDataStoreQueryable<T> source, CancellationToken cancellationToken = default) where T : INumberBase<T>
    {
        var sum = T.Zero;
        await foreach (var item in source.WithCancellation(cancellationToken))
        {
            sum += item;
        }
        return sum;
    }
}
