using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using NeverFoundry.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.Cosmos
{
    /// <summary>
    /// A data store for <see cref="IIdItem"/> instances backed by Azure Cosmos DB.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default interface methods which query for a specific item assumes that the <see
    /// cref="IIdItem.Id"/> property is the partition key for the container. If a different
    /// partition key is used, be sure to use the overload which takes one as a parameter.
    /// </para>
    /// <para>
    /// The default interface methods which retrieve paginated results will only function properly
    /// when getting the first page. For all subsequent pages, use the overload which takes a
    /// continuation token. This token is exposed as a property of the <see
    /// cref="CosmosPagedList{T}"/> which is returned by those overloads, as well as the standard
    /// interface methods (it is a subclass of the base <see cref="PagedList{T}"/> and the result
    /// can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
    /// </para>
    /// </remarks>
    public class CosmosDataStore : IDataStore
    {
        /// <summary>
        /// The <see cref="Microsoft.Azure.Cosmos.Container"/> used for all transactions.
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="CosmosDataStore"/>.
        /// </summary>
        /// <param name="cosmosClient">The <see cref="CosmosClient"/> used for all transactions.</param>
        /// <param name="databaseName">The name of the database used by this <see cref="IDataStore"/>.</param>
        /// <param name="containerName">The name of the container used by this <see cref="IDataStore"/>.</param>
        public CosmosDataStore(
            CosmosClient cosmosClient,
            string databaseName,
            string containerName) => Container = cosmosClient.GetContainer(databaseName, containerName);

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="CosmosDataStore"/> implementation generates a new <see cref="Guid"/>
        /// and returns the result of its <see cref="Guid.ToString()"/> method.
        /// </para>
        /// </remarks>
        public string CreateNewIdFor<T>() where T : IIdItem => Guid.NewGuid().ToString();

        /// <summary>
        /// Creates a new id for an item of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to generate an id. Expected to be an instance of
        /// <see cref="IIdItem"/>, but should not throw an exception even if a different type is
        /// supplied.</param>
        /// <returns>A new id for an item of the given <paramref name="type"/>.</returns>
        /// <remarks>
        /// Whether the id is guaranteed to be unique or not depends on your persistence model and
        /// choice of implementation.
        /// </remarks>
        public string CreateNewIdFor(Type type) => Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>(true).OrderByDescending(selector).FirstOrDefault();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>(true).OrderBy(selector).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemWhere<T>(Expression<Func<T, bool>> condition) where T : class, IIdItem
            => Container.GetItemLinqQueryable<T>(true).FirstOrDefault(condition);

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereAsync<T>(Expression<Func<T, bool>> condition) where T : class, IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>()
                .Where(condition)
                .ToFeedIterator();
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : class, IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>()
                .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>(true)
                    .OrderByDescending(selector)
                    .FirstOrDefault();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>(true)
                    .OrderBy(selector)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>(true).OrderByDescending(selector).FirstOrDefault();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>(true).OrderBy(selector).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructWhere<T>(Expression<Func<T, bool>> condition) where T : struct, IIdItem
            => Container.GetItemLinqQueryable<T>(true).FirstOrDefault(condition);

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        /// <remarks>This is not an async operation in the Cosmos implementation.</remarks>
        public async Task<T?> GetFirstStructWhereAsync<T>(Expression<Func<T, bool>> condition) where T : struct, IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>()
                .ToFeedIterator();
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : struct, IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>()
                .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .FirstOrDefault();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        /// <remarks>This is not an async operation in the Cosmos implementation.</remarks>
        public async Task<T?> GetFirstStructWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            var result = await iterator.ReadNextAsync().ConfigureAwait(false);
            return result?.FirstOrDefault();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            FeedIterator<T>? iterator;
            if (descending)
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderByDescending(selector)
                    .ToFeedIterator();
            }
            else
            {
                iterator = Container.GetItemLinqQueryable<T>()
                    .OrderBy(selector)
                    .ToFeedIterator();
            }
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// <para>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </para>
        /// <para>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </para>
        /// </remarks>
        public T? GetItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return Container.ReadItemAsync<T>(id, new PartitionKey(id)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetItem<T>(string? id, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return Container.ReadItemAsync<T>(id, partitionKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetItem<T>(string? id, string? partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetItem<T>(id);
            }
            return GetItem<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// <para>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </para>
        /// <para>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </para>
        /// </remarks>
        public async Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await Container.ReadItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public async Task<T?> GetItemAsync<T>(string? id, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await Container.ReadItemAsync<T>(id, partitionKey).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public Task<T?> GetItemAsync<T>(string? id, string? partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<T?>(null);
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetItemAsync<T>(id);
            }
            return GetItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Caution: querying an entire container synchronously is a blocking call, and can result
        /// in excessive RUs for a Cosmos datastore. Consider at least using the asynchronous
        /// version (<see cref="GetItemsAsync{T}"/>), or better yet an alternative which avoids
        /// over-querying the data.
        /// </remarks>
        public IReadOnlyList<T> GetItems<T>() where T : IIdItem
            => Container.GetItemLinqQueryable<T>(true).ToList().AsReadOnly();

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Caution: querying an entire container can result in excessive RUs for a Cosmos
        /// datastore. Consider an alternative which avoids over-querying the data.
        /// </remarks>
        public async IAsyncEnumerable<T> GetItemsAsync<T>() where T : IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>().ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Caution: querying an entire container synchronously is a blocking call, and can result
        /// in excessive RUs for a Cosmos datastore. Consider at least using the asynchronous
        /// version (<see cref="GetItemsOrderedByAsync{T, TKey}"/>), or better yet an alternative
        /// which avoids over-querying the data.
        /// </remarks>
        public IReadOnlyList<T> GetItemsOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>(true).OrderByDescending(selector).ToList().AsReadOnly();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>(true).OrderBy(selector).ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Caution: querying an entire container can result in excessive RUs for a Cosmos
        /// datastore. Consider an alternative which avoids over-querying the data.
        /// </remarks>
        public async IAsyncEnumerable<T> GetItemsOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
        {
            FeedIterator<T> iterator;
            {
                if (descending)
                {
                    iterator = Container.GetItemLinqQueryable<T>().OrderByDescending(selector).ToFeedIterator();
                }
                else
                {
                    iterator = Container.GetItemLinqQueryable<T>().OrderBy(selector).ToFeedIterator();
                }
            }
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Caution: querying a container synchronously is a blocking call. Consider using the
        /// asynchronous version (<see cref="GetItemsWhereAsync{T}"/>).
        /// </remarks>
        public IReadOnlyList<T> GetItemsWhere<T>(Expression<Func<T, bool>> condition) where T : IIdItem
            => Container.GetItemLinqQueryable<T>(true).Where(condition).ToList().AsReadOnly();

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereAsync<T>(Expression<Func<T, bool>> condition) where T : IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>().Where(condition).ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>().ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItemsWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
        {
            if (descending)
            {
                return Container.GetItemLinqQueryable<T>(true).Where(condition).OrderByDescending(selector).ToList().AsReadOnly();
            }
            else
            {
                return Container.GetItemLinqQueryable<T>(true).Where(condition).OrderBy(selector).ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
        {
            FeedIterator<T> iterator;
            {
                if (descending)
                {
                    iterator = Container.GetItemLinqQueryable<T>().Where(condition).OrderByDescending(selector).ToFeedIterator();
                }
                else
                {
                    iterator = Container.GetItemLinqQueryable<T>().Where(condition).OrderBy(selector).ToFeedIterator();
                }
            }
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
        {
            FeedIterator<T> iterator;
            {
                if (descending)
                {
                    iterator = Container.GetItemLinqQueryable<T>().OrderByDescending(selector).ToFeedIterator();
                }
                else
                {
                    iterator = Container.GetItemLinqQueryable<T>().OrderBy(selector).ToFeedIterator();
                }
            }
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    if (await condition.Compile().Invoke(item).ConfigureAwait(false))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public IPagedList<T> GetPage<T>(int pageNumber, int pageSize)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();
            return Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                .AsCosmosPagedList(1, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public CosmosPagedList<T> GetPage<T>(int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize }).ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = catchUpIterator.ReadNextAsync().GetAwaiter().GetResult();
                        continuationToken = result.ContinuationToken;
                    }
                }
            }
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize }).ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = iterator.ReadNextAsync().GetAwaiter().GetResult();
                list.AddRange(result);
                continuationToken = result.ContinuationToken;
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public async Task<IPagedList<T>> GetPageAsync<T>(int pageNumber, int pageSize)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize }).ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async Task<CosmosPagedList<T>> GetPageAsync<T>(int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize }).ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = await catchUpIterator.ReadNextAsync().ConfigureAwait(false);
                        if (!(result is null))
                        {
                            continuationToken = result.ContinuationToken;
                        }
                    }
                }
            }
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize }).ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                    continuationToken = result.ContinuationToken;
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public IPagedList<T> GetPageOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();
            return (descending
                ? Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderByDescending(selector)
                : Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderBy(selector))
                .AsCosmosPagedList(1, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public CosmosPagedList<T> GetPageOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0, bool descending = false)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = descending
                        ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .OrderByDescending(selector)
                            .ToFeedIterator()
                        : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .OrderBy(selector)
                            .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = catchUpIterator.ReadNextAsync().GetAwaiter().GetResult();
                        continuationToken = result.ContinuationToken;
                    }
                }
            }
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = iterator.ReadNextAsync().GetAwaiter().GetResult();
                list.AddRange(result);
                continuationToken = result.ContinuationToken;
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public async Task<IPagedList<T>> GetPageOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async Task<CosmosPagedList<T>> GetPageOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0, bool descending = false)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = descending
                        ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .OrderByDescending(selector)
                            .ToFeedIterator()
                        : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .OrderBy(selector)
                            .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = await catchUpIterator.ReadNextAsync().ConfigureAwait(false);
                        if (!(result is null))
                        {
                            continuationToken = result.ContinuationToken;
                        }
                    }
                }
            }
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                    continuationToken = result.ContinuationToken;
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public IPagedList<T> GetPageWhere<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize)
        {
            var count = Container.GetItemLinqQueryable<T>(true).Where(condition).LongCount();
            return Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                .Where(condition)
                .AsCosmosPagedList(1, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public CosmosPagedList<T> GetPageWhere<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0)
        {
            var count = Container.GetItemLinqQueryable<T>(true).Where(condition).LongCount();

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                        .Where(condition)
                        .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = catchUpIterator.ReadNextAsync().GetAwaiter().GetResult();
                        continuationToken = result.ContinuationToken;
                    }
                }
            }
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                .Where(condition)
                .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = iterator.ReadNextAsync().GetAwaiter().GetResult();
                list.AddRange(result);
                continuationToken = result.ContinuationToken;
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public async Task<IPagedList<T>> GetPageWhereAsync<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize)
        {
            var count = await Container.GetItemLinqQueryable<T>().Where(condition).CountAsync().ConfigureAwait(false);
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize })
                .Where(condition)
                .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async Task<CosmosPagedList<T>> GetPageWhereAsync<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0)
        {
            var count = await Container.GetItemLinqQueryable<T>().Where(condition).CountAsync().ConfigureAwait(false);

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                        .Where(condition)
                        .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = await catchUpIterator.ReadNextAsync().ConfigureAwait(false);
                        if (!(result is null))
                        {
                            continuationToken = result.ContinuationToken;
                        }
                    }
                }
            }
            var list = new List<T>();
            var iterator = Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                .Where(condition)
                .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                    continuationToken = result.ContinuationToken;
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public IPagedList<T> GetPageWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();
            return (descending
                ? Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderByDescending(selector)
                : Container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderBy(selector))
                .AsCosmosPagedList(1, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public CosmosPagedList<T> GetPageWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0, bool descending = false)
        {
            var count = Container.GetItemLinqQueryable<T>(true).LongCount();

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = descending
                        ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .Where(condition)
                            .OrderByDescending(selector)
                            .ToFeedIterator()
                        : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .Where(condition)
                            .OrderBy(selector)
                            .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = catchUpIterator.ReadNextAsync().GetAwaiter().GetResult();
                        continuationToken = result.ContinuationToken;
                    }
                }
            }
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = iterator.ReadNextAsync().GetAwaiter().GetResult();
                list.AddRange(result);
                continuationToken = result.ContinuationToken;
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// This method will only function properly when getting the first page. For all subsequent
        /// pages, use the overload which takes a continuation token. This token is exposed as a
        /// property of the <see cref="CosmosPagedList{T}"/> which is returned by that overloads, as
        /// well as this method (it is a subclass of the base <see cref="PagedList{T}"/> and the
        /// result can be cast to the more specific <see cref="CosmosPagedList{T}"/> type).
        /// </remarks>
        public async Task<IPagedList<T>> GetPageWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, null, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count);
        }

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <param name="continuationTokenPage">
        /// The page number on which the current continuation token was last used.
        /// </param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async Task<CosmosPagedList<T>> GetPageWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, string? continuationToken, int continuationTokenPage = 0, bool descending = false)
        {
            var count = await Container.GetItemLinqQueryable<T>().CountAsync().ConfigureAwait(false);

            if (continuationTokenPage >= pageNumber)
            {
                continuationTokenPage = 0;
            }
            if (continuationTokenPage < pageNumber - 1)
            {
                continuationToken = null;
                for (var i = continuationTokenPage; i < pageNumber - 1; i++)
                {
                    var catchUpIterator = descending
                        ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .Where(condition)
                            .OrderByDescending(selector)
                            .ToFeedIterator()
                        : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                            .Where(condition)
                            .OrderBy(selector)
                            .ToFeedIterator();
                    while (catchUpIterator.HasMoreResults)
                    {
                        var result = await catchUpIterator.ReadNextAsync().ConfigureAwait(false);
                        if (!(result is null))
                        {
                            continuationToken = result.ContinuationToken;
                        }
                    }
                }
            }
            var list = new List<T>();
            var iterator = descending
                ? Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderByDescending(selector)
                    .ToFeedIterator()
                : Container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageSize })
                    .Where(condition)
                    .OrderBy(selector)
                    .ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var result = await iterator.ReadNextAsync().ConfigureAwait(false);
                if (!(result is null))
                {
                    list.AddRange(result);
                    continuationToken = result.ContinuationToken;
                }
            }
            return list.AsCosmosPagedList(pageNumber, pageSize, count, continuationToken);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// <para>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </para>
        /// <para>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </para>
        /// </remarks>
        public T? GetStruct<T>(string? id) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return Container.ReadItemAsync<T>(id, new PartitionKey(id)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetStruct<T>(string? id, PartitionKey partitionKey) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return Container.ReadItemAsync<T>(id, partitionKey).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetStruct<T>(string? id, string? partitionKey) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetStruct<T>(id);
            }
            return GetStruct<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// <para>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </para>
        /// <para>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </para>
        /// </remarks>
        public async Task<T?> GetStructAsync<T>(string? id) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await Container.ReadItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public async Task<T?> GetStructAsync<T>(string? id, PartitionKey partitionKey) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return await Container.ReadItemAsync<T>(id, partitionKey).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public Task<T?> GetStructAsync<T>(string? id, string? partitionKey) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult<T?>(null);
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetStructAsync<T>(id);
            }
            return GetStructAsync<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </remarks>
        public bool RemoveItem<T>(string? id) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            Container.DeleteItemAsync<T>(id, new PartitionKey(id)).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public bool RemoveItem<T>(string? id, PartitionKey partitionKey) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            Container.DeleteItemAsync<T>(id, partitionKey).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public bool RemoveItem<T>(string? id, string? partitionKey) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return RemoveItem<T>(id);
            }
            return RemoveItem<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <param name="item">
        /// <para>
        /// The item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method is not implemented in the Cosmos implementation. Use the overload which
        /// takes an ID instead.
        /// </remarks>
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
        public bool RemoveItem(IIdItem? item) => throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </remarks>
        public async Task<bool> RemoveItemAsync<T>(string? id) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            await Container.DeleteItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public async Task<bool> RemoveItemAsync<T>(string? id, PartitionKey partitionKey) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            await Container.DeleteItemAsync<T>(id, partitionKey).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public Task<bool> RemoveItemAsync<T>(string? id, string? partitionKey) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return Task.FromResult(true);
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return RemoveItemAsync<T>(id);
            }
            return RemoveItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <param name="item">
        /// <para>
        /// The item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        /// <remarks>
        /// This method is not implemented in the Cosmos implementation. Use the overload which
        /// takes an ID instead.
        /// </remarks>
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
        public Task<bool> RemoveItemAsync(IIdItem? item) => throw new NotImplementedException();
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            Container.UpsertItemAsync(item).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            Container.UpsertItemAsync(item, partitionKey).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item, string? partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            await Container.UpsertItemAsync(item).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            await Container.UpsertItemAsync(item, partitionKey).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item, string? partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            await Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreStruct<T>(T? item) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            Container.UpsertItemAsync(item).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreStruct<T>(T? item, PartitionKey partitionKey) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            Container.UpsertItemAsync(item, partitionKey).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreStruct<T>(T? item, string? partitionKey) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreStructAsync<T>(T? item) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            await Container.UpsertItemAsync(item).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreStructAsync<T>(T? item, PartitionKey partitionKey) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            await Container.UpsertItemAsync(item, partitionKey).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreStructAsync<T>(T? item, string? partitionKey) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            await Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).ConfigureAwait(false);
            return true;
        }
    }
}
