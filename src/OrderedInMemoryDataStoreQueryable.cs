using System.Linq.Expressions;

namespace Tavenem.DataStorage;

/// <summary>
/// Provides LINQ operations on an <see cref="InMemoryDataStore"/>, after an ordering operation.
/// </summary>
public class OrderedInMemoryDataStoreQueryable<T>(IOrderedEnumerable<T> source)
    : InMemoryDataStoreQueryable<T>(source), IOrderedDataStoreQueryable<T>
{
    /// <summary>
    /// Performs a subsequent ordering of the elements in this <see
    /// cref="IOrderedDataStoreQueryable{T}"/> in the given order.
    /// </summary>
    /// <typeparam name="TKey">The type of the key returned by the function represented by
    /// <paramref name="keySelector"/>.</typeparam>
    /// <param name="keySelector">A function to extract a key from each element.</param>
    /// <param name="descending">Whether results will be ordered in descending order.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted according to a
    /// key.
    /// </returns>
    public IOrderedDataStoreQueryable<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false)
        => descending
            ? new OrderedInMemoryDataStoreQueryable<T>(((IOrderedEnumerable<T>)_source).ThenByDescending(keySelector.Compile()))
            : new OrderedInMemoryDataStoreQueryable<T>(((IOrderedEnumerable<T>)_source).ThenBy(keySelector.Compile()));
}
