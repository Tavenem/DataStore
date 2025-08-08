namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="SkipLast(int)"/>
/// operation.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <remarks>
/// Note: The <see cref="IDataStoreQueryable{TSource}"/> inherits from <see
/// cref="IAsyncEnumerable{T}"/>, which already provides these methods as extensions. This interface
/// is intended for data stores that implement these methods at the data source (e.g. by translating
/// them to native database calls), rather than relying on client-side LINQ evaluation which may
/// result in a more expensive database call than necessary (e.g. retrieving the full list).
/// </remarks>
public interface IDataStoreSkipLastQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Returns a new queryable sequence that contains the elements from source with the last count
    /// elements of the source queryable sequence omitted.
    /// </summary>
    /// <param name="count">
    /// The number of elements to omit from the end of the queryable sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSkipLastQueryable{T}"/> that contains the elements from this source
    /// minus <paramref name="count"/> elements from the end of the queryable sequence.
    /// </returns>
    IDataStoreSkipLastQueryable<TSource> SkipLast(int count);
}
