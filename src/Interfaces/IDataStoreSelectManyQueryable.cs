using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="SelectMany{TCollection, TResult}(Expression{Func{TSource, IEnumerable{TCollection}}},
/// Expression{Func{TSource, TCollection, TResult}})"/> and <see cref="SelectMany{TCollection,
/// TResult}(Expression{Func{TSource, int, IEnumerable{TCollection}}}, Expression{Func{TSource,
/// TCollection, TResult}})"/> operations.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <remarks>
/// Note: The <see cref="IDataStoreQueryable{TSource}"/> inherits from <see
/// cref="IAsyncEnumerable{T}"/>, which already provides these methods as extensions. This interface
/// is intended for data stores that implement these methods at the data source (e.g. by translating
/// them to native database calls), rather than relying on client-side LINQ evaluation which may
/// result in a more expensive database call than necessary.
/// </remarks>
public interface IDataStoreSelectManyQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Projects each element of a sequence to an <see cref="IEnumerable{T}"/> and invokes a result
    /// selector function on each element therein. The resulting values from each intermediate
    /// sequence are combined into a single, one-dimensional sequence and returned.
    /// </summary>
    /// <typeparam name="TCollection">
    /// The type of the intermediate elements collected by the function represented by <paramref
    /// name="collectionSelector"/>.
    /// </typeparam>
    /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
    /// <param name="collectionSelector">
    /// A projection function to apply to each element of the input sequence; the second parameter
    /// of this function represents the index of the source element.
    /// </param>
    /// <param name="resultSelector">
    /// A projection function to apply to each element of each intermediate sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSelectQueryable{T}"/> whose elements are the result of invoking the
    /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
    /// this source and then mapping each of those sequence elements and their corresponding source
    /// element to a result element.
    /// </returns>
    IDataStoreSelectManyQueryable<TResult> SelectMany<TCollection, TResult>(
        Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector,
        Expression<Func<TSource, TCollection, TResult>> resultSelector) where TResult : notnull;

    /// <summary>
    /// Projects each element of a sequence to an <see cref="IEnumerable{T}"/> that incorporates the
    /// index of the source element that produced it. A result selector function is invoked on each
    /// element of each intermediate sequence, and the resulting values are combined into a single,
    /// one-dimensional sequence and returned.
    /// </summary>
    /// <typeparam name="TCollection">
    /// The type of the intermediate elements collected by the function represented by <paramref
    /// name="collectionSelector"/>.
    /// </typeparam>
    /// <typeparam name="TResult">The type of the elements of the resulting sequence.</typeparam>
    /// <param name="collectionSelector">
    /// A projection function to apply to each element of the input sequence; the second parameter
    /// of this function represents the index of the source element.
    /// </param>
    /// <param name="resultSelector">
    /// A projection function to apply to each element of each intermediate sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSelectQueryable{T}"/> whose elements are the result of invoking the
    /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
    /// this source and then mapping each of those sequence elements and their corresponding source
    /// element to a result element.
    /// </returns>
    IDataStoreSelectManyQueryable<TResult> SelectMany<TCollection, TResult>(
        Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector,
        Expression<Func<TSource, TCollection, TResult>> resultSelector) where TResult : notnull;
}
