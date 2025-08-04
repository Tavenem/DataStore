namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="TakeLast(int)"/>
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
public interface IDataStoreTakeLastQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Returns a new queryable sequence that contains the last <paramref name="count"/> elements
    /// from this source.
    /// </summary>
    /// <param name="count">
    /// The number of elements to take from the end of this sequence.
    /// </param>
    /// <returns>
    /// A new queryable sequence that contains the last <paramref name="count"/> elements from this
    /// source.
    /// </returns>
    IDataStoreTakeLastQueryable<TSource> TakeLast(int count);
}
