using System.Numerics;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage;

/// <summary>
/// Extension methods for <see cref="IIdItemDataStore"/> related to <see cref="IDataStoreQueryable{T}"/>.
/// </summary>
public static class DataStoreQueryableExtensions
{
    /// <summary>
    /// Asynchronously computes the average of an <see cref="IDataStoreQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of items in <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IDataStoreQueryable{T}"/> source.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The average of the values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    public static ValueTask<TSource> AverageAsync<TSource>(
        this IDataStoreQueryable<TSource> source,
        CancellationToken cancellationToken = default) where TSource : INumberBase<TSource>
    {
        if (source.Provider is not IDataStore dataStore)
        {
            throw new InvalidOperationException("The source IDataStoreQueryable does not have a valid IDataStore provider.");
        }

        return dataStore.AverageAsync(source, cancellationToken);
    }

    /// <summary>
    /// Asynchronously computes the sum of an <see cref="IDataStoreQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of items in <paramref name="source"/>.</typeparam>
    /// <param name="source">The <see cref="IDataStoreQueryable{T}"/> source.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The sum of the values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    public static ValueTask<TSource> SumAsync<TSource>(
        this IDataStoreQueryable<TSource> source,
        CancellationToken cancellationToken = default) where TSource : INumberBase<TSource>
    {
        if (source.Provider is not IDataStore dataStore)
        {
            throw new InvalidOperationException("The source IDataStoreQueryable does not have a valid IDataStore provider.");
        }
        return dataStore.SumAsync(source, cancellationToken);
    }
}
