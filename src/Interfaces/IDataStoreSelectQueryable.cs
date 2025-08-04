using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="Select{TResult}(Expression{Func{TSource, TResult}})"/> and <see
/// cref="Select{TResult}(Expression{Func{TSource, int, TResult}})"/> operations.
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
public interface IDataStoreSelectQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Projects each element of this sequence into a new form.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <returns>
    /// An <see cref="IDataStoreSelectQueryable{T}"/> whose elements are the result of invoking a
    /// projection function on each element of this source.
    /// </returns>
    IDataStoreSelectQueryable<TResult> Select<TResult>(Expression<Func<TSource, TResult>> selector);

    /// <summary>
    /// Projects each element of this sequence into a new form by incorporating the element's index.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the value returned by the function represented by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <returns>
    /// An <see cref="IDataStoreSelectQueryable{T}"/> whose elements are the result of invoking a
    /// projection function on each element of this source.
    /// </returns>
    IDataStoreSelectQueryable<TResult> Select<TResult>(Expression<Func<TSource, int, TResult>> selector);
}
