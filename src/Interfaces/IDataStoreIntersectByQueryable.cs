using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="IntersectBy{TKey}(IEnumerable{TKey}, Expression{Func{TSource, TKey}},
/// IEqualityComparer{TKey}?)"/> operations.
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
public interface IDataStoreIntersectByQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Produces the set intersection of two sequences according to a specified key selector
    /// function.
    /// </summary>
    /// <param name="source2">
    /// A sequence whose distinct elements that also appear in this sequence are returned.
    /// </param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// A sequence that contains the set intersection of the two sequences.
    /// </returns>
    IDataStoreIntersectByQueryable<TSource> IntersectBy<TKey>(
        IEnumerable<TKey> source2,
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null);
}
