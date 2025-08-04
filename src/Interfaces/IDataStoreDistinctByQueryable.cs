using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="DistinctBy{TKey}(Expression{Func{TSource, TKey}}, IEqualityComparer{TKey}?)"/>
/// operation.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <remarks>
/// Note: The <see cref="IDataStoreQueryable{TSource}"/> inherits from <see
/// cref="IAsyncEnumerable{T}"/>, which already provides this methods as an extension. This
/// interface is intended for data stores that implement the method at the data source (e.g. by
/// translating them to native database calls), rather than relying on client-side LINQ evaluation
/// which may result in a more expensive database call than necessary (e.g. retrieving the full list
/// in order to iterate only the elements which satisfy the condition).
/// </remarks>
public interface IDataStoreDistinctByQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Returns distinct elements from a sequence according to a specified key selector function.
    /// </summary>
    /// <typeparam name="TKey">The type of key to distinguish elements by.</typeparam>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IDataStoreDistinctByQueryable{TSource}"/> that contains distinct elements from
    /// this sequence.
    /// </returns>
    IDataStoreDistinctByQueryable<TSource> DistinctBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey>? comparer = null);
}
