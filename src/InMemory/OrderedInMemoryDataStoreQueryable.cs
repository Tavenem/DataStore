using System.Linq.Expressions;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage.InMemory;

/// <summary>
/// Provides LINQ operations on an <see cref="InMemoryDataStore"/>, after an ordering operation.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <param name="dataStore">The <see cref="InMemoryDataStore"/>.</param>
/// <param name="source">An <see cref="IOrderedQueryable{TSource}"/>.</param>
public class OrderedInMemoryDataStoreQueryable<TSource>(IInMemoryDataStore dataStore, IOrderedQueryable<TSource> source)
    : InMemoryDataStoreQueryable<TSource>(dataStore, source), IOrderedDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Gets the source <see cref="IQueryable{T}"/> for this instance.
    /// </summary>
    public override IQueryable<TSource> Source => OrderedSource;

    /// <summary>
    /// Gets the source <see cref="IOrderedQueryable{T}"/> for this instance.
    /// </summary>
    public IOrderedQueryable<TSource> OrderedSource { get; } = source;

    /// <summary>
    /// Performs a subsequent ordering of the elements in this <see
    /// cref="IOrderedDataStoreQueryable{T}"/> in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function represented by <paramref
    /// name="keySelector"/>.</typeparam>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted according to a key.
    /// </returns>
    public IOrderedDataStoreQueryable<TSource> ThenBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, OrderedSource.ThenBy(keySelector, comparer));

    /// <summary>
    /// Performs a subsequent ordering of the elements in this <see
    /// cref="IOrderedDataStoreQueryable{T}"/> in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function represented by <paramref
    /// name="keySelector"/>.</typeparam>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted in descending order
    /// according to a key.
    /// </returns>
    public IOrderedDataStoreQueryable<TSource> ThenByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, OrderedSource.ThenByDescending(keySelector, comparer));
}
