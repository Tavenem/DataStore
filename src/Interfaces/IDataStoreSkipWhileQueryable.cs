using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="SkipWhile(Expression{Func{TSource, bool}})"/> and <see
/// cref="SkipWhile(Expression{Func{TSource, int, bool}})"/> operations.
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
public interface IDataStoreSkipWhileQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns
    /// the remaining elements.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSkipQueryable{T}"/> that contains elements from this source starting
    /// at the first element in the linear series that does not pass the test specified by
    /// <paramref name="predicate"/>.
    /// </returns>
    IDataStoreSkipWhileQueryable<TSource> SkipWhile(Expression<Func<TSource, bool>> predicate);

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns
    /// the remaining elements. The element's index is used in the logic of the predicate function.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition; the second parameter of this function
    /// represents the index of the source element.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreSkipQueryable{T}"/> that contains elements from this source starting
    /// at the first element in the linear series that does not pass the test specified by <paramref
    /// name="predicate"/>.
    /// </returns>
    IDataStoreSkipWhileQueryable<TSource> SkipWhile(Expression<Func<TSource, int, bool>> predicate);
}
