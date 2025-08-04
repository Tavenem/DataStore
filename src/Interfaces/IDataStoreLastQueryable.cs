using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports various <c>First</c> and
/// <c>FirstOrDefault</c> operations.
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
/// to iterate only the last element).
/// </remarks>
public interface IDataStoreLastQueryable<TSource> : IDataStoreQueryable<TSource>
{
    /// <summary>
    /// Returns the last element of this source, asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The last element in this source, or a default value if the sequence
    /// contains no elements.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> LastAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the last element of this source that satisfies a
    /// specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="default"/> if this source is empty or if no element
    /// passes the test specified by <paramref name="predicate"/>; otherwise, the last element in
    /// source that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> LastAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the last element of this source value if the sequence contains no
    /// elements, asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The last element in this source, or a default value if the sequence
    /// contains no elements.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> LastOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the last element of this source that satisfies a
    /// specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="default"/> if this source is empty or if no element
    /// passes the test specified by <paramref name="predicate"/>; otherwise, the last element in
    /// source that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> LastOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default);
}
