using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports various <c>First</c>,
/// <c>FirstOrDefault</c>, <c>Single</c>, and <c>SingleOrDefault</c> operations.
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
/// to iterate only the first element).
/// </remarks>
public interface IDataStoreFirstQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Asynchronously returns the first element of this source.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The first element in this sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> FirstAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the first element of this source that satisfies a
    /// specified condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="default"/> if this source is empty or if no element
    /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element in
    /// source that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> FirstAsync(Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first element of this source, or a default value if the sequence contains no
    /// elements, asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The first element in this source, or a default value if the sequence contains no elements.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the first element of this source that satisfies a
    /// specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="default"/> if this source is empty or if no element
    /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element in
    /// source that passes the test specified by <paramref name="predicate"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> FirstOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only element of this source, and throws an
    /// exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The single element of the input sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements or more than one element.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only element of this source, and throws an
    /// exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The single element of the input sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains no elements or more than one element.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource> SingleAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only element of this source, or a default
    /// value if the sequence is empty, and throws an exception if there is more than one element in
    /// the sequence.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The single element of the input sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains more than one element.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously returns the only element of this source, or a default
    /// value if the sequence is empty, and throws an exception if there is more than one element in
    /// the sequence.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>The single element of the input sequence.</returns>
    /// <exception cref="InvalidOperationException">
    /// the source contains more than one element.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<TSource?> SingleOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default);
}
