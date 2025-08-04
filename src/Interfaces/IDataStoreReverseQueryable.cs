namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="Reverse"/>
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
/// require multiple iterations and/or higher resource use.
/// </remarks>
public interface IDataStoreReverseQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Inverts the order of the elements in this sequence.
    /// </summary>
    /// <returns>
    /// An <see cref="IDataStoreReverseQueryable{TSource}"/> whose elements correspond to those of
    /// the input sequence in reverse order.
    /// </returns>
    IDataStoreReverseQueryable<TSource> Reverse();
}
