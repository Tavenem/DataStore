using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see
/// cref="Where(Expression{Func{TSource, bool}})"/> and <see cref="Where(Expression{Func{TSource,
/// int, bool}})"/> operations.
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
public interface IDataStoreWhereQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// An <see cref="IDataStoreWhereQueryable{T}"/>that contains elements from this sequence that
    /// satisfy the condition specified by <paramref name="predicate"/>.
    /// </returns>
    IDataStoreWhereQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate);

    /// <summary>
    /// Filters a sequence of values based on a predicate. Each element's index is used in the logic
    /// of the predicate function.
    /// </summary>
    /// <param name="predicate">
    /// A function to test each element for a condition; the second parameter of the function
    /// represents the index of the element in the source sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreWhereQueryable{T}"/>that contains elements from this sequence that
    /// satisfy the condition specified by <paramref name="predicate"/>.
    /// </returns>
    IDataStoreWhereQueryable<TSource> Where(Expression<Func<TSource, int, bool>> predicate);
}
