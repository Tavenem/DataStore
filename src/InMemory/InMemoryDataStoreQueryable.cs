using System.Collections;
using System.Linq.Expressions;
using System.Numerics;
using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage.InMemory;

/// <summary>
/// Provides LINQ operations on an <see cref="InMemoryDataStore"/>.
/// </summary>
/// <typeparam name="TSource">
/// The type of the elements of the source.
/// </typeparam>
/// <param name="dataStore">The <see cref="InMemoryDataStore"/>.</param>
/// <param name="source">An <see cref="IQueryable{TSource}"/>.</param>
public class InMemoryDataStoreQueryable<TSource>(IInMemoryDataStore dataStore, IQueryable<TSource> source)
    : IDataStoreDistinctByQueryable<TSource>,
    IDataStoreDistinctQueryable<TSource>,
    IDataStoreFirstQueryable<TSource>,
    IDataStoreGroupByQueryable<TSource>,
    IDataStoreGroupJoinQueryable<TSource>,
    IDataStoreIntersectQueryable<TSource>,
    IDataStoreIntersectByQueryable<TSource>,
    IDataStoreJoinQueryable<TSource>,
    IDataStoreLastQueryable<TSource>,
    IDataStoreOfTypeQueryable<TSource>,
    IDataStoreOrderableQueryable<TSource>,
    IDataStoreReverseQueryable<TSource>,
    IDataStoreSelectManyQueryable<TSource>,
    IDataStoreSelectQueryable<TSource>,
    IDataStoreSkipLastQueryable<TSource>,
    IDataStoreSkipQueryable<TSource>,
    IDataStoreSkipWhileQueryable<TSource>,
    IDataStoreTakeLastQueryable<TSource>,
    IDataStoreTakeQueryable<TSource>,
    IDataStoreTakeWhileQueryable<TSource>,
    IDataStoreUnionByQueryable<TSource>,
    IDataStoreUnionQueryable<TSource>,
    IDataStoreWhereQueryable<TSource>,
    IDataStoreZipQueryable<TSource>,
    IEnumerable<TSource>
{
    /// <summary>
    /// The <see cref="InMemoryDataStore"/> provider for this queryable.
    /// </summary>
    public IInMemoryDataStore InMemoryProvider { get; } = dataStore;

    /// <inheritdoc/>
    IDataStore IDataStoreQueryable<TSource>.Provider => InMemoryProvider;

    /// <summary>
    /// Gets the source <see cref="IQueryable{T}"/> for this instance.
    /// </summary>
    public virtual IQueryable<TSource> Source { get; } = source;

    /// <inheritdoc />
    public ValueTask<TResult> AverageAsync<TResult>(
        Expression<Func<TSource, TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var func = selector.Compile();
        try
        {
            var sum = TResult.Zero;
            var count = 0L;
            foreach (var item in this)
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
                : new(sum / TResult.CreateChecked(count));
        }
        catch (OverflowException) { }

        // overflow occurred, try less precise method which does not use a total
        try
        {
            var average = TResult.Zero;
            var count = this.LongCount();
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = TResult.CreateChecked(count);

            foreach (var item in this)
            {
                if (func(item) is TResult value)
                {
                    checked
                    {
                        average += value / tCount;
                    }
                }
            }

            return new(average);
        }
        catch (OverflowException)
        {
            throw new OverflowException("The average of the sequence is too large to be represented by the type.");
        }
    }

    /// <inheritdoc />
    public async ValueTask<TResult> AverageAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        try
        {
            var sum = TResult.Zero;
            var count = 0L;
            foreach (var item in this)
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
            var count = this.LongCount();
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = TResult.CreateChecked(count);

            foreach (var item in this)
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

    /// <inheritdoc />
    public IDataStoreDistinctQueryable<TSource> Distinct(IEqualityComparer<TSource>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Distinct(comparer));

    /// <inheritdoc />
    public IDataStoreDistinctByQueryable<TSource> DistinctBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.DistinctBy(keySelector, comparer));

    /// <inheritdoc />
    public ValueTask<TSource> FirstAsync(CancellationToken cancellationToken = default)
        => new(Source.First());

    /// <inheritdoc />
    public ValueTask<TSource> FirstAsync(Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default)
        => new(Source.First(predicate));

    /// <inheritdoc />
    public ValueTask<TSource?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        => new(Source.FirstOrDefault());

    /// <inheritdoc />
    public ValueTask<TSource?> FirstOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        => new(Source.FirstOrDefault(predicate));

    /// <inheritdoc />
    IAsyncEnumerator<TSource> IAsyncEnumerable<TSource>.GetAsyncEnumerator(CancellationToken cancellationToken)
        => Source.ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

    /// <inheritdoc />
    public IEnumerator<TSource> GetEnumerator() => Source.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => Source.GetEnumerator();

    /// <inheritdoc />
    public ValueTask<IEnumerator<TSource>> GetEnumeratorAsync(CancellationToken cancellationToken = default) => new(GetEnumerator());

    /// <inheritdoc />
    public ValueTask<IPagedList<TSource>> GetPageAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = InMemoryProvider.Data.Values.OfType<TSource>().LongCount();
        if (totalCount == 0)
        {
            return new(new PagedList<TSource>([], pageNumber, pageSize, 0));
        }

        if (pageNumber <= 0
            || pageSize <= 0)
        {
            return new(new PagedList<TSource>([], pageNumber, pageSize, totalCount));
        }

        var skip = (pageNumber - 1) * pageSize;
        if (skip >= totalCount)
        {
            return new(new PagedList<TSource>([], pageNumber, pageSize, totalCount));
        }

        var items = ((IEnumerable<TSource>)this).Skip(skip).Take(pageSize).ToList();
        return new(new PagedList<TSource>(items, pageNumber, pageSize, totalCount));
    }

    /// <inheritdoc />
    public IDataStoreGroupByQueryable<TResult> GroupBy<TKey, TResult>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.GroupBy(keySelector, resultSelector, comparer));

    /// <inheritdoc />
    public IDataStoreGroupByQueryable<TResult> GroupBy<TKey, TElement, TResult>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TSource, TElement>> elementSelector,
        Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.GroupBy(keySelector, elementSelector, resultSelector, comparer));

    /// <inheritdoc />
    public IDataStoreGroupByQueryable<IGrouping<TKey, TSource>> GroupBy<TKey>(
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<IGrouping<TKey, TSource>>(InMemoryProvider, Source.GroupBy(keySelector, comparer));

    /// <inheritdoc />
    public IDataStoreGroupByQueryable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(
        Expression<Func<TSource, TKey>> keySelector,
        Expression<Func<TSource, TElement>> elementSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<IGrouping<TKey, TElement>>(InMemoryProvider, Source.GroupBy(keySelector, elementSelector, comparer));

    /// <inheritdoc />
    public IDataStoreGroupJoinQueryable<TResult> GroupJoin<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource, IEnumerable<TInner>, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));

    /// <inheritdoc />
    public IDataStoreIntersectQueryable<TSource> Intersect(IEnumerable<TSource> source2, IEqualityComparer<TSource>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Intersect(source2, comparer));

    /// <inheritdoc />
    public IDataStoreIntersectByQueryable<TSource> IntersectBy<TKey>(
        IEnumerable<TKey> source2,
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.IntersectBy(source2, keySelector, comparer));

    /// <inheritdoc />
    public IDataStoreJoinQueryable<TResult> Join<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource, TInner, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.Join(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));

    /// <inheritdoc />
    public ValueTask<TSource> LastAsync(CancellationToken cancellationToken = default)
        => new(Source.Last());

    /// <inheritdoc />
    public ValueTask<TSource> LastAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        => new(Source.Last(predicate));

    /// <inheritdoc />
    public ValueTask<TSource?> LastOrDefaultAsync(CancellationToken cancellationToken = default)
        => new(Source.LastOrDefault());

    /// <inheritdoc />
    public ValueTask<TSource?> LastOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        => new(Source.LastOrDefault(predicate));

    /// <inheritdoc />
    public IDataStoreJoinQueryable<TResult> LeftJoin<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource, TInner?, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.LeftJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));

    /// <inheritdoc />
    public ValueTask<TResult?> MaxAsync<TResult>(
        Expression<Func<TSource, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var func = selector.Compile();
        TResult? max = default;
        foreach (var item in this)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = func(item);
            if (max is null || Comparer<TResult>.Default.Compare(value, max) > 0)
            {
                max = value;
            }
        }
        return new(max);
    }

    /// <inheritdoc />
    public async ValueTask<TResult?> MaxAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        TResult? max = default;
        foreach (var item in this)
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

    /// <inheritdoc />
    public ValueTask<TResult?> MinAsync<TResult>(
        Expression<Func<TSource, TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        var func = selector.Compile();
        TResult? min = default;
        foreach (var item in this)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = func(item);
            if (min is null || Comparer<TResult>.Default.Compare(value, min) < 0)
            {
                min = value;
            }
        }
        return new(min);
    }

    /// <inheritdoc />
    public async ValueTask<TResult?> MinAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellationToken = default)
    {
        TResult? max = default;
        foreach (var item in this)
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

    /// <inheritdoc />
    public IDataStoreOfTypeQueryable<TResult> OfType<TResult>(JsonTypeInfo<TResult>? typeInfo = null) where TResult : TSource
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.OfType<TResult>());

    /// <inheritdoc />
    public IOrderedDataStoreQueryable<TSource> Order(IComparer<TSource>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Order());

    /// <inheritdoc />
    public IOrderedDataStoreQueryable<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.OrderBy(keySelector, comparer));

    /// <inheritdoc />
    public IOrderedDataStoreQueryable<TSource> OrderByDescending<TKey>(Expression<Func<TSource, TKey>> keySelector, IComparer<TKey>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.OrderByDescending(keySelector, comparer));

    /// <inheritdoc />
    public IOrderedDataStoreQueryable<TSource> OrderDescending(IComparer<TSource>? comparer = null)
        => new OrderedInMemoryDataStoreQueryable<TSource>(InMemoryProvider, comparer is null ? Source.OrderDescending() : Source.OrderDescending(comparer));

    /// <inheritdoc />
    public IDataStoreReverseQueryable<TSource> Reverse()
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Reverse());

    /// <inheritdoc />
    public IDataStoreJoinQueryable<TResult> RightJoin<TInner, TKey, TResult>(
        IEnumerable<TInner> inner,
        Expression<Func<TSource, TKey>> outerKeySelector,
        Expression<Func<TInner, TKey>> innerKeySelector,
        Expression<Func<TSource?, TInner, TResult>> resultSelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.RightJoin(inner, outerKeySelector, innerKeySelector, resultSelector, comparer));

    /// <inheritdoc />
    public IDataStoreSelectQueryable<TResult> Select<TResult>(Expression<Func<TSource, TResult>> selector)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.Select(selector));

    /// <inheritdoc />
    public IDataStoreSelectQueryable<TResult> Select<TResult>(Expression<Func<TSource, int, TResult>> selector)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.Select(selector));

    /// <inheritdoc />
    public IDataStoreSelectManyQueryable<TResult> SelectMany<TCollection, TResult>(
        Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector,
        Expression<Func<TSource, TCollection, TResult>> resultSelector)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.SelectMany(collectionSelector, resultSelector));

    /// <inheritdoc />
    public IDataStoreSelectManyQueryable<TResult> SelectMany<TCollection, TResult>(
        Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector,
        Expression<Func<TSource, TCollection, TResult>> resultSelector)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.SelectMany(collectionSelector, resultSelector));

    /// <inheritdoc />
    public ValueTask<TSource> SingleAsync(CancellationToken cancellationToken = default)
        => new(Source.Single());

    /// <inheritdoc />
    public ValueTask<TSource> SingleAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        => new(Source.Single(predicate));

    /// <inheritdoc />
    public ValueTask<TSource?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
        => new(Source.SingleOrDefault());

    /// <inheritdoc />
    public ValueTask<TSource?> SingleOrDefaultAsync(
        Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        => new(Source.SingleOrDefault(predicate));

    /// <inheritdoc />
    public IDataStoreSkipQueryable<TSource> Skip(int count)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Skip(count));

    /// <inheritdoc />
    public IDataStoreSkipLastQueryable<TSource> SkipLast(int count)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.SkipLast(count));

    /// <inheritdoc />
    public IDataStoreSkipWhileQueryable<TSource> SkipWhile(Expression<Func<TSource, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.SkipWhile(predicate));

    /// <inheritdoc />
    public IDataStoreSkipWhileQueryable<TSource> SkipWhile(Expression<Func<TSource, int, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.SkipWhile(predicate));

    /// <inheritdoc />
    public ValueTask<TResult> SumAsync<TResult>(
        Expression<Func<TSource, TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var func = selector.Compile();
        var sum = TResult.Zero;
        foreach (var item in this)
        {
            if (func(item) is TResult value)
            {
                sum += value;
            }
        }
        return new(sum);
    }

    /// <inheritdoc />
    public async ValueTask<TResult> SumAsync<TResult>(
        Func<TSource, CancellationToken, ValueTask<TResult?>> selector,
        CancellationToken cancellationToken = default) where TResult : INumberBase<TResult>
    {
        var sum = TResult.Zero;
        foreach (var item in this)
        {
            if (await selector(item, cancellationToken) is TResult value)
            {
                sum += value;
            }
        }
        return sum;
    }

    /// <inheritdoc />
    public IDataStoreTakeQueryable<TSource> Take(int count)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Take(count));

    /// <inheritdoc />
    public IDataStoreTakeQueryable<TSource> Take(Range range)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Take(range));

    /// <inheritdoc />
    public IDataStoreTakeLastQueryable<TSource> TakeLast(int count)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.TakeLast(count));

    /// <inheritdoc />
    public IDataStoreTakeWhileQueryable<TSource> TakeWhile(Expression<Func<TSource, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.TakeWhile(predicate));

    /// <inheritdoc />
    public IDataStoreTakeWhileQueryable<TSource> TakeWhile(Expression<Func<TSource, int, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.TakeWhile(predicate));

    /// <inheritdoc />
    public ValueTask<(bool Success, int Count)> TryGetNonEnumeratedCountAsync(CancellationToken cancellationToken = default)
        => new((true, InMemoryProvider.Data.Values.OfType<TSource>().Count()));

    /// <inheritdoc />
    public ValueTask<(bool Success, long Count)> TryGetNonEnumeratedLongCountAsync(CancellationToken cancellationToken = default)
        => new((true, InMemoryProvider.Data.Values.OfType<TSource>().LongCount()));

    /// <inheritdoc />
    public IDataStoreUnionQueryable<TSource> Union(IEnumerable<TSource> source2, IEqualityComparer<TSource>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Union(source2, comparer));

    /// <inheritdoc />
    public IDataStoreUnionByQueryable<TSource> UnionBy<TKey>(
        IEnumerable<TSource> source2,
        Expression<Func<TSource, TKey>> keySelector,
        IEqualityComparer<TKey>? comparer = null)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.UnionBy(source2, keySelector, comparer));

    /// <inheritdoc />
    public IDataStoreWhereQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Where(predicate));

    /// <inheritdoc />
    public IDataStoreWhereQueryable<TSource> Where(Expression<Func<TSource, int, bool>> predicate)
        => new InMemoryDataStoreQueryable<TSource>(InMemoryProvider, Source.Where(predicate));

    /// <inheritdoc />
    public IDataStoreZipQueryable<TResult> Zip<TSecond, TResult>(
        IEnumerable<TSecond> source2,
        Expression<Func<TSource, TSecond, TResult>> resultSelector)
        => new InMemoryDataStoreQueryable<TResult>(InMemoryProvider, Source.Zip(source2, resultSelector));

    /// <inheritdoc />
    public IDataStoreZipQueryable<(TSource First, TSecond Second)> Zip<TSecond>(IEnumerable<TSecond> source2)
        => new InMemoryDataStoreQueryable<(TSource First, TSecond Second)>(InMemoryProvider, Source.Zip(source2));

    /// <inheritdoc />
    public IDataStoreZipQueryable<(TSource First, TSecond Second, TThird Third)> Zip<TSecond, TThird>(
        IEnumerable<TSecond> source2,
        IEnumerable<TThird> source3)
        => new InMemoryDataStoreQueryable<(TSource First, TSecond Second, TThird Third)>(InMemoryProvider, Source.Zip(source2, source3));
}
