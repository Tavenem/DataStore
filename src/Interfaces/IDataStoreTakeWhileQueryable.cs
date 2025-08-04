using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="TakeWhile(Expression{Func{TSource, bool}})"/> and <see
/// cref="TakeWhile(Expression{Func{TSource, int, bool}})"/> operations.
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
public interface IDataStoreTakeWhileQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreTakeWhileQueryable{T}"/> that contains elements from the input
    /// sequence occurring before the element at which the test specified by <paramref
    /// name="predicate"/> no longer passes.
    /// </returns>
    IDataStoreTakeWhileQueryable<TSource> TakeWhile(Expression<Func<TSource, bool>> predicate);

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true. The element's
    /// index is used in the logic of the predicate function.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition; the second parameter of the function
    /// represents the index of the element in the source sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreTakeWhileQueryable{T}"/> that contains elements from the input
    /// sequence occurring before the element at which the test specified by <paramref
    /// name="predicate"/> no longer passes.
    /// </returns>
    IDataStoreTakeWhileQueryable<TSource> TakeWhile(Expression<Func<TSource, int, bool>> predicate);
}
