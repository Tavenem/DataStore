using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// Provides LINQ operations on an <see cref="IIdItemDataStore"/> implementation's data source, after
/// an ordering operation.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
public interface IOrderedDataStoreQueryable<TSource> : IDataStoreQueryable<TSource>
{
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
    IOrderedDataStoreQueryable<TSource> ThenBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null);

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
    IOrderedDataStoreQueryable<TSource> ThenByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null);
}
