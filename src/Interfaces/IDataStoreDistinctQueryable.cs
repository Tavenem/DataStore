namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="Distinct(IEqualityComparer{TSource}?)"/> operation.
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
public interface IDataStoreDistinctQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Returns distinct elements from a sequence by using a specified equality comparer to compare
    /// values.
    /// </summary>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
    /// <returns>
    /// An <see cref="IDataStoreDistinctQueryable{TSource}"/> that contains distinct elements.
    /// </returns>
    IDataStoreDistinctQueryable<TSource> Distinct(IEqualityComparer<TSource>? comparer = null);
}
