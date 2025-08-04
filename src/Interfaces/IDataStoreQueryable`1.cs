using System.Linq.Expressions;
using System.Numerics;

namespace Tavenem.DataStorage.Interfaces;

/// <summary>
/// Provides LINQ operations on an <see cref="IIdItemDataStore"/> implementation's data.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
public interface IDataStoreQueryable<TSource> : IAsyncEnumerable<TSource>
{
    /// <summary>
    /// The <see cref="IDataStore"/> provider for this queryable.
    /// </summary>
    IDataStore Provider { get; }

    /// <summary>
    /// Asynchronously computes the average of this source that is obtained by invoking a projection
    /// function on each element of the input sequence.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The average of the projected values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult> AverageAsync<TResult>(
        Expression<Func<TSource, TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var func = selector.Compile();
        try
        {
            var sum = TResult.Zero;
            var count = 0L;
            await foreach (var item in this.WithCancellation(cancellationToken))
            {
                if (func(item) is TResult value)
                {
                    checked
                    {
                        sum += value;
                    }
                    count++;
                }
            }
            return count == 0
                ? throw new InvalidOperationException("sequence contains no elements")
                : sum / TResult.CreateChecked(count);
        }
        catch (OverflowException) { }

        // overflow occurred, try less precise method which does not use a total
        try
        {
            var average = TResult.Zero;
            var count = await this.LongCountAsync(cancellationToken);
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = TResult.CreateChecked(count);

            await foreach (var item in this.WithCancellation(cancellationToken))
            {
                if (func(item) is TResult value)
                {
                    checked
                    {
                        average += value / tCount;
                    }
                }
            }

            return average;
        }
        catch (OverflowException)
        {
            throw new OverflowException("The average of the sequence is too large to be represented by the type.");
        }
    }

    /// <summary>
    /// Asynchronously computes the average of this source that is obtained by invoking a projection
    /// function on each element of the input sequence.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The average of the projected values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult> AverageAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        try
        {
            var sum = TResult.Zero;
            var count = 0L;
            await foreach (var item in this.WithCancellation(cancellationToken))
            {
                if (await selector(item, cancellationToken) is TResult value)
                {
                    checked
                    {
                        sum += value;
                    }
                    count++;
                }
            }
            return count == 0
                ? throw new InvalidOperationException("sequence contains no elements")
                : sum / TResult.CreateChecked(count);
        }
        catch (OverflowException) { }

        // overflow occurred, try less precise method which does not use a total
        try
        {
            var average = TResult.Zero;
            var count = await this.LongCountAsync(cancellationToken);
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = TResult.CreateChecked(count);

            await foreach (var item in this.WithCancellation(cancellationToken))
            {
                if (await selector(item, cancellationToken) is TResult value)
                {
                    checked
                    {
                        average += value / tCount;
                    }
                }
            }

            return average;
        }
        catch (OverflowException)
        {
            throw new OverflowException("The average of the sequence is too large to be represented by the type.");
        }
    }

    /// <summary>
    /// Returns an <see cref="IEnumerator{T}"/> for this source. The enumerator provides a simple
    /// way to access all the contents of the collection.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IEnumerable{T}" />.</returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    /// <remarks>
    /// Note: the default implementation uses <see
    /// cref="IAsyncEnumerable{T}.GetAsyncEnumerator(CancellationToken)"/> to iterate all elements
    /// and collect them in a <see cref="List{T}"/>, then returns an enumerator for that list.
    /// Prefer to use <see cref="IAsyncEnumerable{T}.GetAsyncEnumerator(CancellationToken)"/>
    /// directly when possible.
    /// </remarks>
    async ValueTask<IEnumerator<TSource>> GetEnumeratorAsync(CancellationToken cancellationToken = default)
    {
        var results = new List<TSource>();
        var asyncEnumerator = GetAsyncEnumerator(cancellationToken);
        while (await asyncEnumerator.MoveNextAsync())
        {
            results.Add(asyncEnumerator.Current);
            cancellationToken.ThrowIfCancellationRequested();
        }
        return results.GetEnumerator();
    }

    /// <summary>
    /// Asynchronously gets a number of items from this source equal to <paramref
    /// name="pageSize"/>, after skipping <paramref name="pageNumber"/>-1 multiples of that amount.
    /// </summary>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// An <see cref="IPagedList{T}"/> of items from the source.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<IPagedList<TSource>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (gotCount, totalCount) = await TryGetNonEnumeratedLongCountAsync(cancellationToken);
        if (gotCount && totalCount == 0)
        {
            return new PagedList<TSource>([], pageNumber, pageSize, 0);
        }

        if (pageNumber <= 0
            || pageSize <= 0)
        {
            return new PagedList<TSource>([], pageNumber, pageSize, gotCount ? totalCount : null);
        }

        var skip = (pageNumber - 1) * pageSize;
        if (gotCount && skip >= totalCount)
        {
            return new PagedList<TSource>([], pageNumber, pageSize, totalCount);
        }

        var items = new List<TSource>();

        var take = pageSize;
        var queryable = this;
        if (queryable is IDataStoreSkipQueryable<TSource> skipQueryable)
        {
            queryable = skipQueryable.Skip(skip);

            if (queryable is IDataStoreTakeQueryable<TSource> takeQueryable)
            {
                queryable = takeQueryable.Take(pageSize);
                items = await queryable.ToListAsync(cancellationToken);
                return new PagedList<TSource>(items, pageNumber, pageSize, gotCount ? totalCount : null);
            }

            await foreach (var item in queryable)
            {
                items.Add(item);
                take--;
                if (take <= 0)
                {
                    break;
                }
            }
            return new PagedList<TSource>(items, pageNumber, pageSize, gotCount ? totalCount : null);
        }

        await foreach (var item in queryable)
        {
            if (skip > 0)
            {
                skip--;
                continue;
            }
            if (take > 0)
            {
                items.Add(item);
                take--;
            }
            if (take <= 0)
            {
                break;
            }
        }
        return new PagedList<TSource>(items, pageNumber, pageSize, gotCount ? totalCount : null);
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of this source and returns the
    /// maximum resulting value.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The maximum value of in the sequence.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult?> MaxAsync<TResult>(
        Expression<Func<TSource, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var func = selector.Compile();
        TResult? max = default;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = func(item);
            if (max is null || Comparer<TResult>.Default.Compare(value, max) > 0)
            {
                max = value;
            }
        }
        return max;
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of this source and returns the
    /// maximum resulting value.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The maximum value of in the sequence.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult?> MaxAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        TResult? max = default;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = await selector(item, cancellationToken);
            if (max is null || Comparer<TResult>.Default.Compare(value, max) > 0)
            {
                max = value;
            }
        }
        return max;
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of this source and returns the
    /// minimum resulting value.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The minimum value of the source.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult?> MinAsync<TResult>(
        Expression<Func<TSource, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var func = selector.Compile();
        TResult? min = default;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = func(item);
            if (min is null || Comparer<TResult>.Default.Compare(value, min) < 0)
            {
                min = value;
            }
        }
        return min;
    }

    /// <summary>
    /// Asynchronously invokes a projection function on each element of this source and returns the
    /// minimum resulting value.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The minimum value of the source.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult?> MinAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        TResult? max = default;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = await selector(item, cancellationToken);
            if (max is null || Comparer<TResult>.Default.Compare(value, max) < 0)
            {
                max = value;
            }
        }
        return max;
    }

    /// <summary>
    /// Asynchronously computes the sum of this source that is obtained by invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult> SumAsync<TResult>(
        Expression<Func<TSource, TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var func = selector.Compile();
        var sum = TResult.Zero;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            if (func(item) is TResult value)
            {
                sum += value;
            }
        }
        return sum;
    }

    /// <summary>
    /// Asynchronously computes the sum of this source that is obtained by
    /// invoking a projection function on each element of the input sequence.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of value projected by <paramref name="selector"/>.
    /// </typeparam>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The sum of the projected values.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    async ValueTask<TResult> SumAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var sum = TResult.Zero;
        await foreach (var item in this.WithCancellation(cancellationToken))
        {
            if (await selector(item, cancellationToken) is TResult value)
            {
                sum += value;
            }
        }
        return sum;
    }

    /// <summary>
    /// Attempts to determine the number of elements in this sequence without forcing an
    /// enumeration.
    /// </summary>
    /// <returns>
    /// A tuple with two elements: the first is <see langword="true"/> if the count of source can be
    /// determined without enumeration; otherwise, <see langword="false"/>. The second contains the
    /// number of elements in this source, or 0 if the count couldn't be determined without
    /// enumeration.
    /// </returns>
    /// <remarks>
    /// When possible, implementations will query the data store for a count, rather than fetch the
    /// full list of elements in order to count them client-side. If the data store itself must get
    /// the list of items in order to obtain a count, this is considered an implementation detail
    /// and should not result in this method returning <see langword="false"/> for the first element
    /// of the tuple.
    /// </remarks>
    ValueTask<(bool Success, int Count)> TryGetNonEnumeratedCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to determine the number of elements in this sequence without forcing an
    /// enumeration.
    /// </summary>
    /// <returns>
    /// A tuple with two elements: the first is <see langword="true"/> if the count of source can be
    /// determined without enumeration; otherwise, <see langword="false"/>. The second contains the
    /// number of elements in this source as a <see cref="long"/>, or 0 if the count couldn't be
    /// determined without enumeration.
    /// </returns>
    /// <remarks>
    /// When possible, implementations will query the data store for a count, rather than fetch the
    /// full list of elements in order to count them client-side. If the data store itself must get
    /// the list of items in order to obtain a count, this is considered an implementation detail
    /// and should not result in this method returning <see langword="false"/> for the first element
    /// of the tuple.
    /// </remarks>
    ValueTask<(bool Success, long Count)> TryGetNonEnumeratedLongCountAsync(CancellationToken cancellationToken = default);
}
