using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports <c>Join</c> operations.
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
public interface IDataStoreJoinQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified <see
    /// cref="IEqualityComparer{T}"/> is used to compare keys.
    /// </summary>
    /// <typeparam name="TInner">The type of the elements of the other sequence.</typeparam>
    /// <typeparam name="TKey">
    /// The type of the keys returned by the key selector functions.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result value returned by <paramref name="resultSelector"/>.
    /// </typeparam>
    /// <param name="inner">The sequence to join to this sequence.</param>
    /// <param name="outerKeySelector">
    /// A function to extract the join key from each element of this sequence.
    /// </param>
    /// <param name="innerKeySelector">
    /// A function to extract the join key from each element of the other sequence.
    /// </param>
    /// <param name="resultSelector">
    /// A function to create a result element from two matching elements.
    /// </param>
    /// <param name="comparer">A comparer to hash and compare keys.</param>
    /// <returns>
    /// An <see cref="IDataStoreJoinQueryable{T}"/> that has elements of type
    /// <typeparamref name="TResult"/> obtained by performing an inner join on two sequences.
    /// </returns>
    IDataStoreJoinQueryable<TResult> Join<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource, TInner, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null) where TResult : notnull;

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified <see
    /// cref="IEqualityComparer{T}"/> is used to compare keys.
    /// </summary>
    /// <typeparam name="TInner">The type of the elements of the other sequence.</typeparam>
    /// <typeparam name="TKey">
    /// The type of the keys returned by the key selector functions.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result value returned by <paramref name="resultSelector"/>.
    /// </typeparam>
    /// <param name="inner">The sequence to join to this sequence.</param>
    /// <param name="outerKeySelector">
    /// A function to extract the join key from each element of this sequence.
    /// </param>
    /// <param name="innerKeySelector">
    /// A function to extract the join key from each element of the other sequence.
    /// </param>
    /// <param name="resultSelector">
    /// A function to create a result element from two matching elements.
    /// </param>
    /// <param name="comparer">A comparer to hash and compare keys.</param>
    /// <returns>
    /// An <see cref="IDataStoreJoinQueryable{T}"/> that has elements of type
    /// <typeparamref name="TResult"/> obtained by performing a left outer join on two sequences.
    /// </returns>
    IDataStoreJoinQueryable<TResult> LeftJoin<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource, TInner?, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null) where TResult : notnull;

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys. A specified <see
    /// cref="IEqualityComparer{T}"/> is used to compare keys.
    /// </summary>
    /// <typeparam name="TInner">The type of the elements of the other sequence.</typeparam>
    /// <typeparam name="TKey">
    /// The type of the keys returned by the key selector functions.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the result value returned by <paramref name="resultSelector"/>.
    /// </typeparam>
    /// <param name="inner">The sequence to join to this sequence.</param>
    /// <param name="outerKeySelector">
    /// A function to extract the join key from each element of this sequence.
    /// </param>
    /// <param name="innerKeySelector">
    /// A function to extract the join key from each element of the other sequence.
    /// </param>
    /// <param name="resultSelector">
    /// A function to create a result element from two matching elements.
    /// </param>
    /// <param name="comparer">A comparer to hash and compare keys.</param>
    /// <returns>
    /// An <see cref="IDataStoreJoinQueryable{T}"/> that has elements of type
    /// <typeparamref name="TResult"/> obtained by performing a right outer join on two sequences.
    /// </returns>
    IDataStoreJoinQueryable<TResult> RightJoin<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource?, TInner, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null) where TResult : notnull;
}
