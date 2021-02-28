using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using NeverFoundry.DataStorage.Cosmos;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// Provides LINQ operations on a <see cref="CosmosDataStore"/>.
    /// </summary>
    public class CosmosDataStoreQueryable<T> : IDataStoreQueryable<T>
    {
        private protected readonly Container _container;
        private protected readonly IQueryable<T> _source;

        private protected string? _continuationToken;
        private protected int _lastPage;

        /// <summary>
        /// Initializes a new instance of <see cref="CosmosDataStoreQueryable{T}"/>.
        /// </summary>
        public CosmosDataStoreQueryable(Container container, IQueryable<T> source)
        {
            _container = container;
            _source = source;
        }

        /// <summary>
        /// Determines whether this <see cref="IDataStoreQueryable{T}"/> contains any elements.
        /// </summary>
        /// <returns><see langword="true"/> if the source sequence contains any elements; otherwise,
        /// <see langword="false"/>.</returns>
        public bool Any()
        {
            var iterator = _container.GetItemQueryIterator<T>(_source.ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = 1 });
            if (iterator.HasMoreResults)
            {
                var results = iterator.ReadNextAsync().GetAwaiter().GetResult();
                return results.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether any element of this <see cref="IDataStoreQueryable{T}"/> satisfies a
        /// condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        public bool Any(Expression<Func<T, bool>> predicate)
        {
            var iterator = _container.GetItemQueryIterator<T>(_source.Where(predicate).ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = 1 });
            if (iterator.HasMoreResults)
            {
                var results = iterator.ReadNextAsync().GetAwaiter().GetResult();
                return results.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously determines whether this <see cref="IDataStoreQueryable{T}"/> contains any
        /// elements.
        /// </summary>
        /// <returns><see langword="true"/> if the source sequence contains any elements; otherwise,
        /// <see langword="false"/>.</returns>
        public async Task<bool> AnyAsync()
        {
            var iterator = _container.GetItemQueryIterator<T>(_source.ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = 1 });
            if (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync().ConfigureAwait(false);
                return results.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously determines whether any element of this <see
        /// cref="IDataStoreQueryable{T}"/> satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var iterator = _container.GetItemQueryIterator<T>(_source.Where(predicate).ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = 1 });
            if (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync().ConfigureAwait(false);
                return results.Count > 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Asynchronously determines whether any element of this <see
        /// cref="IDataStoreQueryable{T}"/> satisfies a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns><see langword="true"/> if any elements in the source sequence pass the test in
        /// the specified predicate; otherwise,
        /// <see langword="false"/>.</returns>
        public async Task<bool> AnyAsync(Func<T, ValueTask<bool>> predicate)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await predicate.Invoke(item).ConfigureAwait(false))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}" /> as an asynchronous
        /// operation.
        /// </summary>
        /// <returns>An <see cref="IAsyncEnumerable{T}" />.</returns>
        /// <remarks>
        /// Caution: querying an entire container can result in excessive RUs for a Cosmos
        /// datastore. Consider an alternative which avoids over-querying the data.
        /// </remarks>
        public async IAsyncEnumerable<T> AsAsyncEnumerable()
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}" />.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}" />.</returns>
        /// <remarks>
        /// Caution: querying an entire container synchronously is a blocking call, and can result
        /// in excessive RUs for a Cosmos datastore. Consider at least using the asynchronous
        /// version (<see cref="AsAsyncEnumerable"/>), or better yet an alternative which avoids
        /// over-querying the data.
        /// </remarks>
        public IEnumerable<T> AsEnumerable()
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in iterator.ReadNextAsync().GetAwaiter().GetResult())
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns the number of elements in this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The number of elements in this <see cref="IDataStoreQueryable{T}"/>.</returns>
        /// <exception cref="OverflowException">
        /// The number of elements in source is larger than <see cref="int.MaxValue"/>.
        /// </exception>
        public int Count() => _source.CountAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously returns the number of elements in this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The number of elements in this <see cref="IDataStoreQueryable{T}"/>.</returns>
        /// <exception cref="OverflowException">
        /// The number of elements in source is larger than <see cref="int.MaxValue"/>.
        /// </exception>
        public async Task<int> CountAsync() => await _source.CountAsync().ConfigureAwait(false);

        /// <summary>
        /// Returns the first element of this <see cref="IDataStoreQueryable{T}" />, or a default
        /// value if the sequence contains no elements.
        /// </summary>
        /// <returns>
        /// The first element in this <see cref="IDataStoreQueryable{T}" />, or a default value if
        /// the sequence contains no elements.
        /// </returns>
        public T? FirstOrDefault()
        {
            var iterator = _source.ToFeedIterator();
            if (iterator.HasMoreResults)
            {
                var results = iterator.ReadNextAsync().GetAwaiter().GetResult();
                return results.FirstOrDefault();
            }
            return default;
        }

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
        public T? FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var iterator = _source.Where(predicate).ToFeedIterator();
            if (iterator.HasMoreResults)
            {
                var results = iterator.ReadNextAsync().GetAwaiter().GetResult();
                return results.FirstOrDefault();
            }
            return default;
        }

        /// <summary>
        /// Returns the first element of this <see cref="IDataStoreQueryable{T}" />, or a default
        /// value if the sequence contains no elements, asynchronously.
        /// </summary>
        /// <returns>
        /// The first element in this <see cref="IDataStoreQueryable{T}" />, or a default value if
        /// the sequence contains no elements.
        /// </returns>
        public async Task<T?> FirstOrDefaultAsync()
        {
            var iterator = _source.ToFeedIterator();
            if (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync().ConfigureAwait(false);
                return results.FirstOrDefault()!;
            }
            return default!;
        }

        /// <summary>
        /// Asynchronously returns the first element of this <see cref="IDataStoreQueryable{T}"/>
        /// that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(TSource) if this <see cref="IDataStoreQueryable{T}"/> is empty or if no element
        /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element
        /// in source that passes the test specified by <paramref name="predicate"/>.
        /// </returns>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var iterator = _source.Where(predicate).ToFeedIterator();
            if (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync().ConfigureAwait(false);
                return results.FirstOrDefault()!;
            }
            return default!;
        }

        /// <summary>
        /// Asynchronously returns the first element of this <see cref="IDataStoreQueryable{T}"/>
        /// that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// default(TSource) if this <see cref="IDataStoreQueryable{T}"/> is empty or if no element
        /// passes the test specified by <paramref name="predicate"/>; otherwise, the first element
        /// in source that passes the test specified by <paramref name="predicate"/>.
        /// </returns>
        public async Task<T?> FirstOrDefaultAsync(Func<T, ValueTask<bool>> predicate)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await predicate.Invoke(item).ConfigureAwait(false))
                    {
                        return item;
                    }
                }
            }
            return default!;
        }

        /// <summary>
        /// Gets a number of items from this <see cref="IDataStoreQueryable{T}" /> equal to
        /// <paramref name="pageSize" />, after skipping <paramref name="pageNumber" /> multiples of
        /// that amount.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}" /> of items from this <see
        /// cref="IDataStoreQueryable{T}" />.</returns>
        public IPagedList<T> GetPage(int pageNumber, int pageSize)
        {
            CosmosPagedList<T> page;
            if (!string.IsNullOrEmpty(_continuationToken)
                && pageNumber == _lastPage + 1)
            {
                page = _container.GetItemQueryIterator<T>(_source.ToQueryDefinition(), _continuationToken, new QueryRequestOptions { MaxItemCount = pageSize + 1 })
                      .AsCosmosPagedListAsync(pageNumber, pageSize).GetAwaiter().GetResult();
            }
            else
            {
                page = _container.GetItemQueryIterator<T>(_source.Skip((pageNumber - 1) * pageSize).ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = pageSize + 1 })
                      .AsCosmosPagedListAsync(pageNumber, pageSize).GetAwaiter().GetResult();
            }
            _continuationToken = page.ContinuationToken;
            _lastPage = pageNumber;
            return page;
        }

        /// <summary>
        /// Asynchronously gets a number of items from this <see cref="IDataStoreQueryable{T}"/>
        /// equal to <paramref name="pageSize"/>, after skipping <paramref name="pageNumber"/>
        /// multiples of that amount.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items from this <see
        /// cref="IDataStoreQueryable{T}"/>.</returns>
        public async Task<IPagedList<T>> GetPageAsync(int pageNumber, int pageSize)
        {
            CosmosPagedList<T> page;
            if (!string.IsNullOrEmpty(_continuationToken)
                && pageNumber == _lastPage + 1)
            {
                page = await _container.GetItemQueryIterator<T>(_source.ToQueryDefinition(), _continuationToken, new QueryRequestOptions { MaxItemCount = pageSize + 1 })
                    .AsCosmosPagedListAsync(pageNumber, pageSize).ConfigureAwait(false);
            }
            else
            {
                page = await _container.GetItemQueryIterator<T>(_source.Skip((pageNumber - 1) * pageSize).ToQueryDefinition(), null, new QueryRequestOptions { MaxItemCount = pageSize + 1 })
                    .AsCosmosPagedListAsync(pageNumber, pageSize).ConfigureAwait(false);
            }
            _continuationToken = page.ContinuationToken;
            _lastPage = pageNumber;
            return page;
        }

        /// <summary>
        /// Returns the maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The maximum value of this <see cref="IDataStoreQueryable{T}"/>.</returns>
        public T? Max() => _source.MaxAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously returns the maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>
        /// The maximum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        public async Task<T?> MaxAsync() => await _source.MaxAsync().ConfigureAwait(false);

        /// <summary>
        /// Returns the minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>The minimum value of this <see cref="IDataStoreQueryable{T}"/>.</returns>
        public T? Min() => _source.MinAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously returns the minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <returns>
        /// The minimum value of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        public async Task<T?> MinAsync() => await _source.MinAsync().ConfigureAwait(false);

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
        public IDataStoreQueryable<TResult> OfType<TResult>()
            => new CosmosDataStoreQueryable<TResult>(_container, _source.OfType<TResult>());

        /// <summary>
        /// Sorts the elements of this <see cref="IDataStoreQueryable{T}" /> in ascending order
        /// according to a key.
        /// </summary>
        /// <param name="keySelector">A function to extract a key from an element.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IDataStoreQueryable{T}" />.</returns>
        public IOrderedDataStoreQueryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false)
            => descending
                ? new OrderedCosmosDataStoreQueryable<T>(_container, _source.OrderByDescending(keySelector))
                : new OrderedCosmosDataStoreQueryable<T>(_container, _source.OrderBy(keySelector));

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
        public IDataStoreQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
            => new CosmosDataStoreQueryable<TResult>(_container, _source.Select(selector));

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
        public async IAsyncEnumerable<TResult> SelectAsync<TResult>(Func<T, ValueTask<TResult>> selector)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return await selector.Invoke(item).ConfigureAwait(false);
                }
            }
        }

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
        public IDataStoreQueryable<TResult> SelectMany<TResult>(Expression<Func<T, IEnumerable<TResult>>> selector)
            => new CosmosDataStoreQueryable<TResult>(_container, _source.SelectMany(selector));

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
        public IDataStoreQueryable<TResult> SelectMany<TCollection, TResult>(
            Expression<Func<T, IEnumerable<TCollection>>> collectionSelector,
            Expression<Func<T, TCollection, TResult>> resultSelector)
            => new CosmosDataStoreQueryable<TResult>(_container, _source.SelectMany(collectionSelector, resultSelector));

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
        public async IAsyncEnumerable<TResult> SelectManyAsync<TResult>(Func<T, IAsyncEnumerable<TResult>> selector)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    await foreach (var subItem in selector.Invoke(item))
                    {
                        yield return subItem;
                    }
                }
            }
        }

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
        public async IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, ValueTask<TResult>> resultSelector)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    foreach (var subItem in collectionSelector.Invoke(item))
                    {
                        yield return await resultSelector.Invoke(item, subItem).ConfigureAwait(false);
                    }
                }
            }
        }

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
        public async IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IAsyncEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, TResult> resultSelector)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    await foreach (var subItem in collectionSelector.Invoke(item))
                    {
                        yield return resultSelector.Invoke(item, subItem);
                    }
                }
            }
        }

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
        public async IAsyncEnumerable<TResult> SelectManyAsync<TCollection, TResult>(
            Func<T, IAsyncEnumerable<TCollection>> collectionSelector,
            Func<T, TCollection, ValueTask<TResult>> resultSelector)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    await foreach (var subItem in collectionSelector.Invoke(item))
                    {
                        yield return await resultSelector.Invoke(item, subItem).ConfigureAwait(false);
                    }
                }
            }
        }

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
        public IDataStoreQueryable<T> Skip(int count)
            => new CosmosDataStoreQueryable<T>(_container, _source.Skip(count));

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of this <see
        /// cref="IDataStoreQueryable{T}"/>.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>
        /// An <see cref="IDataStoreQueryable{T}"/> that contains the specified number of elements
        /// from the start of this <see cref="IDataStoreQueryable{T}"/>.
        /// </returns>
        public IDataStoreQueryable<T> Take(int count)
            => new CosmosDataStoreQueryable<T>(_container, _source.Take(count));

        /// <summary>
        /// Enumerates the results of this <see cref="IDataStoreQueryable{T}" /> and returns them as
        /// a <see cref="IReadOnlyList{T}" />.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}" />.</returns>
        public IReadOnlyList<T> ToList()
        {
            var list = new List<T>();
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in iterator.ReadNextAsync().GetAwaiter().GetResult())
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Asynchronously enumerates the results of this <see cref="IDataStoreQueryable{T}"/> and
        /// returns them as a <see cref="IReadOnlyList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}"/>.</returns>
        public async Task<IReadOnlyList<T>> ToListAsync()
        {
            var list = new List<T>();
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// Filters this <see cref="IDataStoreQueryable{T}" /> based on a <paramref
        /// name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IDataStoreQueryable{T}" />.</returns>
        public IDataStoreQueryable<T> Where(Expression<Func<T, bool>> predicate)
            => new CosmosDataStoreQueryable<T>(_container, _source.Where(predicate));

        /// <summary>
        /// Filters this <see cref="IDataStoreQueryable{T}" /> based on an asynchronous <paramref
        /// name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}" />.</returns>
        public async IAsyncEnumerable<T> WhereAsync(Func<T, ValueTask<bool>> predicate)
        {
            var iterator = _source.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await predicate.Invoke(item).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
