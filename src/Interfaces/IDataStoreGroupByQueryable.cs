using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports various <c>GroupBy</c> operations.
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
public interface IDataStoreGroupByQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates
    /// a result value from each group and its key. Keys are compared by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function represented in <paramref name="keySelector"/>.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result value returned by <paramref name="resultSelector"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="resultSelector">
    /// A function to create a result value from each group.
    /// </param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IDataStoreGroupByQueryable{T}"/> that has a type argument of <typeparamref
    /// name="TResult"/> and where each element represents a projection over a group and its key.
    /// </returns>
    IDataStoreGroupByQueryable<TResult> GroupBy<TKey, TResult>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null) where TResult : notnull;

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and creates
    /// a result value from each group and its key. Keys are compared by using a specified comparer
    /// and the elements of each group are projected by using a specified function.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function represented in <paramref name="keySelector"/>.
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the elements in each <see cref="IGrouping{TKey, TElement}"/>.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result value returned by <paramref name="resultSelector"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">
    /// A function to map each source element to an element in an <see cref="IGrouping{TKey,
    /// TElement}"/>.
    /// </param>
    /// <param name="resultSelector">
    /// A function to create a result value from each group.
    /// </param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <see cref="IDataStoreGroupByQueryable{T}"/> that has a type argument of <typeparamref
    /// name="TResult"/> and where each element represents a projection over a group and its key.
    /// </returns>
    IDataStoreGroupByQueryable<TResult> GroupBy<TKey, TElement, TResult>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TSource, TElement>> elementSelector,
        Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null) where TResult : notnull;

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function and
    /// compares the keys by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function represented in <paramref name="keySelector"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <c>IDataStoreGroupByQueryable&lt;IGrouping&lt;TKey,TSource>></c> where each <see
    /// cref="IGrouping{TKey, TElement}"/> contains a sequence of objects and a key.
    /// </returns>
    IDataStoreGroupByQueryable<IGrouping<TKey, TSource>> GroupBy<TKey>(
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null);

    /// <summary>
    /// Groups the elements of a sequence and projects the elements for each group by using a
    /// specified function. Key values are compared by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the key returned by the function represented in <paramref name="keySelector"/>.
    /// </typeparam>
    /// <typeparam name="TElement">
    /// The type of the elements in each <see cref="IGrouping{TKey, TElement}"/>.
    /// </typeparam>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="elementSelector">
    /// A function to map each source element to an element in an <see cref="IGrouping{TKey,
    /// TElement}"/>.
    /// </param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>
    /// An <c>IDataStoreGroupByQueryable&lt;IGrouping&lt;TKey,TElement>></c> where each <see
    /// cref="IGrouping{TKey, TElement}"/> contains a sequence of objects of type <typeparamref
    /// name="TElement"/> and a key.
    /// </returns>
    IDataStoreGroupByQueryable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TSource, TElement>> elementSelector,
        IEqualityComparer<TKey>? comparer = null);
}
