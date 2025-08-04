using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="Order(IComparer{TSource}?)"/>, <see cref="OrderBy{TKey}(Expression{Func{TSource, TKey}},
/// IComparer{TKey}?)"/>, <see cref="OrderByDescending{TKey}(Expression{Func{TSource, TKey}},
/// IComparer{TKey}?)"/>, and <see cref="OrderDescending(IComparer{TSource}?)"/> operations.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <remarks>
/// Note: The <see cref="IDataStoreQueryable{TSource}"/> inherits from <see
/// cref="IAsyncEnumerable{T}"/>, which already provides these methods as extensions. This interface
/// is intended for data stores that implement these methods at the data source (e.g. by translating
/// them to native database calls), rather than relying on client-side LINQ evaluation which may
/// require multiple iterations and/or higher resource use.
/// </remarks>
public interface IDataStoreOrderableQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Sorts the elements of this sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare elements.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted.
    /// </returns>
    IOrderedDataStoreQueryable<TSource> Order(IComparer<TSource>? comparer = null);

    /// <summary>
    /// Sorts the elements of this sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function that is represented by <paramref
    /// name="keySelector"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare elements.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted according to a key.
    /// </returns>
    IOrderedDataStoreQueryable<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null);

    /// <summary>
    /// Sorts the elements of this sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function that is represented by <paramref
    /// name="keySelector"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare elements.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted according to a key.
    /// </returns>
    IOrderedDataStoreQueryable<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null);

    /// <summary>
    /// Sorts the elements of this sequence in descending order by using a specified comparer.
    /// </summary>
    /// <param name="comparer">An <see cref="IComparer{T}"/> to compare elements.</param>
    /// <returns>
    /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted.
    /// </returns>
    IOrderedDataStoreQueryable<TSource> OrderDescending(IComparer<TSource>? comparer = null);
}
