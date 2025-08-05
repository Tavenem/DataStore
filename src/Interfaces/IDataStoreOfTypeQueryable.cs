using System.Text.Json.Serialization.Metadata;

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
    /// <param name="typeInfo">
    /// <para>
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="TResult"/>.
    /// </para>
    /// <para>
    /// This parameter is useful only for data sources which (de)serialize to/from JSON, but the
    /// parameter is provided so that callers without knowledge of the underlying storage
    /// implementation may supply the <see cref="JsonTypeInfo{T}"/> (when available) in case it
    /// might be necessary.
    /// </para>
    /// </param>
    /// <returns>
    /// A collection that contains the elements from this source that have type <typeparamref
    /// name="TResult"/>.
    /// </returns>
    IDataStoreOfTypeQueryable<TResult> OfType<TResult>(JsonTypeInfo<TResult>? typeInfo = null) where TResult : TSource;
}
