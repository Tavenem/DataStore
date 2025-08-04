using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="UnionBy{TKey}(IEnumerable{TSource}, Expression{Func{TSource, TKey}},
/// IEqualityComparer{TKey}?)"/> operation.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <remarks>
/// Note: The <see cref="IDataStoreQueryable{TSource}"/> inherits from <see
/// cref="IAsyncEnumerable{T}"/>, which already provides this method as an extension. This interface
/// is intended for data stores that implement the method at the data source (e.g. by translating
/// them to native database calls), rather than relying on client-side LINQ evaluation which may
/// result in a more expensive database call than necessary.
/// </remarks>
public interface IDataStoreUnionByQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Produces the set union of two sequences by using a specified <see
    /// cref="IEqualityComparer{T}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of key to identify elements by.</typeparam>
    /// <param name="source2">
    /// A sequence whose distinct elements form the second set for the union operation.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IDataStoreUnionQueryable{T}"/> that contains the elements from both input
    /// sequences, excluding duplicates.
    /// </returns>
    IDataStoreUnionByQueryable<TSource> UnionBy<TKey>(
        IEnumerable<TSource> source2,
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null);
}
