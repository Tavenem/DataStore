namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="Take(int)"/> and
/// <see cref="Take(Range)"/> operations.
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
public interface IDataStoreTakeQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Returns a specified number of contiguous elements from the start of this sequence.
    /// </summary>
    /// <param name="count">
    /// The number of elements to return.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreTakeQueryable{T}"/> that contains the specified number of elements
    /// from the start of this source.
    /// </returns>
    IDataStoreTakeQueryable<TSource> Take(int count);

    /// <summary>
    /// Returns a specified range of contiguous elements from a sequence.
    /// </summary>
    /// <param name="range">
    /// The range of elements to return, which has start and end indexes either from the start or
    /// the end.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreTakeQueryable{T}"/> that contains the specified range of elements
    /// from this sequence.
    /// </returns>
    IDataStoreTakeQueryable<TSource> Take(Range range);
}
