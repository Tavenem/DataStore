namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="OfType{TResult}"/>
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
/// result in a more expensive database call than necessary (e.g. retrieving the full list in order
/// to iterate only the matching elements).
/// </remarks>
public interface IDataStoreOfTypeQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Filters the elements of this <see cref="IDataStoreOfTypeQueryable{TSource}"/> based on a
    /// specified type.
    /// </summary>
    /// <typeparam name="TResult">The type to filter the elements of the sequence on.</typeparam>
    /// <returns>
    /// A collection that contains the elements from this source that have type <typeparamref
    /// name="TResult"/>.
    /// </returns>
    IDataStoreOfTypeQueryable<TResult> OfType<TResult>() where TResult : TSource;
}
