namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="Skip(int)"/>
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
public interface IDataStoreSkipQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining
    /// elements.
    /// </summary>
    /// <param name="count">
    /// The number of elements to skip before returning the remaining elements.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSkipQueryable{T}"/> that contains elements that occur after the
    /// specified index in the input sequence.
    /// </returns>
    IDataStoreSkipQueryable<TSource> Skip(int count);
}
