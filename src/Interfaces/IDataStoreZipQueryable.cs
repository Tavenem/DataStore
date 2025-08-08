using System.Linq.Expressions;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// An <see cref="IDataStoreQueryable{TSource}"/> which supports the <see cref="Zip{TSecond,
/// TThird}(IEnumerable{TSecond}, IEnumerable{TThird})"/>, <see
/// cref="Zip{TSecond}(IEnumerable{TSecond})"/>, and <see cref="Zip{TSecond,
/// TResult}(IEnumerable{TSecond}, Expression{Func{TSource, TSecond, TResult}})"/> operations.
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
public interface IDataStoreZipQueryable<TSource> : IDataStoreQueryable<TSource>
    where TSource : notnull
{
    /// <summary>
    /// Merges two sequences by using the specified predicate function.
    /// </summary>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the result sequence.</typeparam>
    /// <param name="source2">The second sequence to merge.</param>
    /// <param name="resultSelector">
    /// A function that specifies how to merge the elements from the two sequences.
    /// </param>
    /// <returns>
    /// An <see cref="IDataStoreZipQueryable{T}"/> that contains merged elements of two input
    /// sequences.
    /// </returns>
    IDataStoreZipQueryable<TResult> Zip<TSecond, TResult>(
        IEnumerable<TSecond> source2,
        Expression<Func<TSource, TSecond, TResult>> resultSelector) where TResult : notnull;

    /// <summary>
    /// Produces a sequence of tuples with elements from the three specified sequences.
    /// </summary>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <param name="source2">The second sequence to merge.</param>
    /// <returns>
    /// A sequence of tuples with elements taken from the first and second sequences, in that order.
    /// </returns>
    IDataStoreZipQueryable<(TSource First, TSecond Second)> Zip<TSecond>(IEnumerable<TSecond> source2);

    /// <summary>
    /// Produces a sequence of tuples with elements from the three specified sequences.
    /// </summary>
    /// <typeparam name="TSecond">The type of the elements of the second input sequence.</typeparam>
    /// <typeparam name="TThird">The type of the elements of the third input sequence.</typeparam>
    /// <param name="source2">The second sequence to merge.</param>
    /// <param name="source3">The third sequence to merge.</param>
    /// <returns>
    /// A sequence of tuples with elements taken from the first, second and third sequences, in that
    /// order.
    /// </returns>
    IDataStoreZipQueryable<(TSource First, TSecond Second, TThird Third)> Zip<TSecond, TThird>(
        IEnumerable<TSecond> source2,
        IEnumerable<TThird> source3);
}
