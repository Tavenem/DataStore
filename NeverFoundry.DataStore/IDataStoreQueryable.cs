using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// Provides LINQ operations on an <see cref="IDataStore"/> implementation's data source.
    /// </summary>
    public interface IDataStoreQueryable<T>
    {
        /// <summary>
        /// Determines whether this <see cref="IDataStoreQueryable{T}"/> contains any elements.
        /// </summary>
        /// <returns><see langword="true"/> if the source sequence contains any elements; otherwise,
        /// <see langword="false"/>.</returns>
        bool Any();

        /// <summary>
        /// Determines whether any element of this <see cref="IDataStoreQueryable{T}"/> satisfies a
        /// condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        bool Any(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously determines whether this <see cref="IDataStoreQueryable{T}"/> contains any
        /// elements.
        /// </summary>
        /// <returns><see langword="true"/> if the source sequence contains any elements; otherwise,
        /// <see langword="false"/>.</returns>
        Task<bool> AnyAsync();

        /// <summary>
        /// Asynchronously determines whether any element of this <see
        /// cref="IDataStoreQueryable{T}"/> satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously determines whether any element of this <see
        /// cref="IDataStoreQueryable{T}"/> satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        Task<bool> AnyAsync(Func<T, ValueTask<bool>> predicate);

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}"/> as an asynchronous
        /// operation.
        /// </summary>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/>.</returns>
        IAsyncEnumerable<T> AsAsyncEnumerable();

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/>.</returns>
        IEnumerable<T> AsEnumerable();

        /// <summary>
        /// Returns the number of elements in this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The number of elements in this <see cref="IDataStoreQueryable{T}"/>.</returns>
        /// <exception cref="OverflowException">
        /// The number of elements in source is larger than <see cref="int.MaxValue"/>.
        /// </exception>
        int Count();

        /// <summary>
        /// Asynchronously returns the number of elements in this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The number of elements in this <see cref="IDataStoreQueryable{T}"/>.</returns>
        /// <exception cref="OverflowException">
        /// The number of elements in source is larger than <see cref="int.MaxValue"/>.
        /// </exception>
        Task<int> CountAsync();

        /// <summary>
        /// Returns the first element of this <see cref="IDataStoreQueryable{T}"/>, or a default
        /// value if the sequence contains no elements.
        /// </summary>
        /// <returns>
        /// The first element in this <see cref="IDataStoreQueryable{T}"/>, or a default value if
        /// the sequence contains no elements.
        /// </returns>
        [return: MaybeNull]
        T FirstOrDefault();

        /// <summary>
        /// Returns the first element of this <see cref="IDataStoreQueryable{T}"/> that satisfies a
        /// specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(TSource) if this <see cref="IDataStoreQueryable{T}"/> is empty or if no element
        /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element
        /// in source that passes the test specified by <paramref name="predicate"/>.
        /// </returns>
        [return: MaybeNull]
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <para>
        /// Asynchronously returns the first element of this <see cref="IDataStoreQueryable{T}"/>,
        /// or a default value if the sequence contains no elements.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// <para>
        /// The first element in this <see cref="IDataStoreQueryable{T}"/>, or a default value if
        /// the sequence contains no elements.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </returns>
        Task<T> FirstOrDefaultAsync();

        /// <summary>
        /// <para>
        /// Asynchronously returns the first element of this <see cref="IDataStoreQueryable{T}"/>
        /// that satisfies a specified condition or a default value if no such element is found.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// <para>
        /// default(TSource) if this <see cref="IDataStoreQueryable{T}"/> is empty or if no element
        /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element
        /// in source that passes the test specified by <paramref name="predicate"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// <para>
        /// Asynchronously returns the first element of this <see cref="IDataStoreQueryable{T}"/>
        /// that satisfies a specified condition or a default value if no such element is found.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// <para>
        /// default(TSource) if this <see cref="IDataStoreQueryable{T}"/> is empty or if no element
        /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element
        /// in source that passes the test specified by <paramref name="predicate"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </returns>
        Task<T> FirstOrDefaultAsync(Func<T, ValueTask<bool>> predicate);

        /// <summary>
        /// Gets a number of items from this <see cref="IDataStoreQueryable{T}"/> equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items from this <see
        /// cref="IDataStoreQueryable{T}"/>.</returns>
        IPagedList<T> GetPage(int pageNumber, int pageSize);

        /// <summary>
        /// Asynchronously gets a number of items from this <see cref="IDataStoreQueryable{T}"/>
        /// equal to <paramref name="pageSize"/>, after skipping <paramref name="pageNumber"/>
        /// multiples of that amount.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items from this <see
        /// cref="IDataStoreQueryable{T}"/>.</returns>
        Task<IPagedList<T>> GetPageAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Returns the maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The maximum value of this <see cref="IDataStoreQueryable{T}"/>.</returns>
        [return: MaybeNull]
        T Max();

        /// <summary>
        /// <para>
        /// Asynchronously returns the maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// <para>
        /// The maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </returns>
        Task<T> MaxAsync();

        /// <summary>
        /// Returns the minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The minimum value of this <see cref="IDataStoreQueryable{T}"/>.</returns>
        [return: MaybeNull]
        T Min();

        /// <summary>
        /// <para>
        /// Asynchronously returns the minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </summary>
        /// <returns>
        /// <para>
        /// The minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </para>
        /// <para>
        /// Note: the return value may be <see langword="null"/>. The nullability features of the
        /// language currently do not support indicating possible <see langword="null"/> values in
        /// the result of a <see cref="Task{TResult}"/>.
        /// </para>
        /// </returns>
        Task<T> MinAsync();

        /// <summary>
        /// Filters the elements of this <see cref="IDataStoreQueryable{T}"/> based on a specified
        /// type.
        /// </summary>
        /// <typeparam name="TResult">The type to filter the elements of the sequence
        /// on.</typeparam>
        /// <returns>
        /// A collection that contains the elements from source that have type <typeparamref
        /// name="TResult"/>.
        /// </returns>
        IDataStoreQueryable<TResult> OfType<TResult>();

        /// <summary>
        /// Sorts the elements of this <see cref="IDataStoreQueryable{T}"/> in ascending order
        /// according to a key.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/>.</returns>
        IOrderedDataStoreQueryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the function represented by
        /// selector.</typeparam>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> whose elements are the result of invoking a
        /// projection function on each element of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        IDataStoreQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> into a new form.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the function represented by
        /// selector.</typeparam>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking a
        /// projection function on each element of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        IAsyncEnumerable<TResult> SelectAsync<TResult>(Func<T, ValueTask<TResult>> selector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the elements of the sequence returned by the function represented by
        /// <paramref name="selector"/>.
        /// </typeparam>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> whose elements are the result of invoking a
        /// one-to-many projection function on each element of the input sequence.
        /// </returns>
        IDataStoreQueryable<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/> and invokes a result selector function on each element therein.
        /// The resulting values from each intermediate sequence are combined into a single,
        /// one-dimensional sequence and returned.
        /// </summary>
        /// <typeparam name="TCollection">
        /// The type of the intermediate elements collected by the function represented by
        /// <paramref name="collectionSelector"/>.
        /// </typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting
        /// sequence.</typeparam>
        /// <param name="collectionSelector">A projection function to apply to each element of the
        /// input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each
        /// intermediate sequence.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> whose elements are the result of invoking the
        /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
        /// source and then mapping each of those sequence elements and their corresponding source
        /// element to a result element.
        /// </returns>
        IDataStoreQueryable<TResult> SelectMany<TCollection, TResult>(
            Expression<Func<T, IEnumerable<TCollection>>> collectionSelector,
            Expression<Func<T, TCollection, TResult>> resultSelector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the elements of the sequence returned by the function represented by
        /// <paramref name="selector"/>.
        /// </typeparam>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> whose elements are the result of invoking a
        /// one-to-many projection function on each element of the input sequence.
        /// </returns>
        IAsyncEnumerable<TResult> SelectManyAsync<TResult>(Func<T, IAsyncEnumerable<TResult>> selector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/>
        /// and invokes a result selector function on each element therein. The resulting values
        /// from each intermediate sequence are combined into a single, one-dimensional sequence and
        /// returned.
        /// </summary>
        /// <typeparam name="TCollection">
        /// The type of the intermediate elements collected by the function represented by
        /// <paramref name="collectionSelector"/>.
        /// </typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting
        /// sequence.</typeparam>
        /// <param name="collectionSelector">A projection function to apply to each element of the
        /// input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each
        /// intermediate sequence.</param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking the
        /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
        /// source and then mapping each of those sequence elements and their corresponding source
        /// element to a result element.
        /// </returns>
        IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, ValueTask<TResult>> resultSelector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/> and invokes a result selector function on each element therein.
        /// The resulting values from each intermediate sequence are combined into a single,
        /// one-dimensional sequence and returned.
        /// </summary>
        /// <typeparam name="TCollection">
        /// The type of the intermediate elements collected by the function represented by
        /// <paramref name="collectionSelector"/>.
        /// </typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting
        /// sequence.</typeparam>
        /// <param name="collectionSelector">A projection function to apply to each element of the
        /// input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each
        /// intermediate sequence.</param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking the
        /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
        /// source and then mapping each of those sequence elements and their corresponding source
        /// element to a result element.
        /// </returns>
        IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IAsyncEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, TResult> resultSelector);

        /// <summary>
        /// Projects each element of this <see cref="IDataStoreQueryable{T}"/> to an <see
        /// cref="IEnumerable{T}"/>
        /// and invokes a result selector function on each element therein. The resulting values
        /// from each intermediate sequence are combined into a single, one-dimensional sequence and
        /// returned.
        /// </summary>
        /// <typeparam name="TCollection">
        /// The type of the intermediate elements collected by the function represented by
        /// <paramref name="collectionSelector"/>.
        /// </typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting
        /// sequence.</typeparam>
        /// <param name="collectionSelector">A projection function to apply to each element of the
        /// input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each
        /// intermediate sequence.</param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> whose elements are the result of invoking the
        /// one-to-many projection function <paramref name="collectionSelector"/> on each element of
        /// source and then mapping each of those sequence elements and their corresponding source
        /// element to a result element.
        /// </returns>
        IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IAsyncEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, ValueTask<TResult>> resultSelector);

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining
        /// elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining
        /// elements.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> that contains elements that occur after the
        /// specified index in the input sequence.
        /// </returns>
        IDataStoreQueryable<T> Skip(int count);

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of this <see
        /// cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> that contains the specified number of elements
        /// from the start of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        IDataStoreQueryable<T> Take(int count);

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}"/> and returns them as
        /// a <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/>.</returns>
        IReadOnlyList<T> ToList();

        /// <summary>
        /// Asynchronously enumerates the results of this <see cref="IDataStoreQueryable{T}"/> and
        /// returns them as a <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/>.</returns>
        Task<IReadOnlyList<T>> ToListAsync();

        /// <summary>
        /// Filters this <see cref="IDataStoreQueryable{T}"/> based on a <paramref
        /// name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/>.</returns>
        IDataStoreQueryable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Filters this <see cref="IDataStoreQueryable{T}"/> based on an asynchronous <paramref
        /// name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/>.</returns>
        IAsyncEnumerable<T> WhereAsync(Func<T, ValueTask<bool>> predicate);
    }
}
