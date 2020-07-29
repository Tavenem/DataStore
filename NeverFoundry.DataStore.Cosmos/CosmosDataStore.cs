using LazyCache;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IAppCache _cache = new CachingService();

        /// <summary>
        /// The <see cref="Microsoft.Azure.Cosmos.Container"/> used for all transactions.
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// <para>
        /// Sets the default period after which cached items are considered stale.
        /// </para>
        /// <para>
        /// This defaults to ten minutes for <see cref="CosmosDataStore"/>.
        /// </para>
        /// </summary>
        public TimeSpan DefaultCacheTimeout { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// <para>
        /// Indicates whether this <see cref="IDataStore"/> implementation allows items to be
        /// cached.
        /// </para>
        /// <para>
        /// This is <see langword="true"/> for <see cref="CosmosDataStore"/>.
        /// </para>
        /// </summary>
        public bool SupportsCaching => true;

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
        public string CreateNewIdFor<T>() where T : class, IIdItem => Guid.NewGuid().ToString();

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
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        public T? GetItem<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return _cache.GetOrAdd(
                id,
                () => Container.ReadItemAsync<T>(id, new PartitionKey(id)).GetAwaiter().GetResult().Resource,
                cacheTimeout ?? DefaultCacheTimeout);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        public T? GetItem<T>(string? id, PartitionKey partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return _cache.GetOrAdd(
                id,
                () => Container.ReadItemAsync<T>(id, partitionKey).GetAwaiter().GetResult(),
                cacheTimeout ?? DefaultCacheTimeout);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        public T? GetItem<T>(string? id, string? partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetItem<T>(id, cacheTimeout);
            }
            return GetItem<T>(id, new PartitionKey(partitionKey), cacheTimeout);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// <para>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </para>
        /// <para>
        /// The <see cref="IIdItem.Id"/> is used as the partition key. If this is not the case, use
        /// one of the overloads which accepts a partition key.
        /// </para>
        /// </remarks>
        public async ValueTask<T?> GetItemAsync<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return await _cache.GetOrAddAsync(
                id,
                async () => (await Container.ReadItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false)).Resource,
                cacheTimeout ?? DefaultCacheTimeout)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        public async ValueTask<T?> GetItemAsync<T>(string? id, PartitionKey partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return await _cache.GetOrAddAsync(
                id,
                async () => (await Container.ReadItemAsync<T>(id, partitionKey).ConfigureAwait(false)).Resource,
                cacheTimeout ?? DefaultCacheTimeout)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        public ValueTask<T?> GetItemAsync<T>(string? id, string? partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return new ValueTask<T?>(default(T?));
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return GetItemAsync<T>(id, cacheTimeout);
            }
            return GetItemAsync<T>(id, new PartitionKey(partitionKey), cacheTimeout);
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
        public IReadOnlyList<T> GetItems<T>() where T : class, IIdItem
            => Container.GetItemLinqQueryable<T>(true).Where(x => x.IdItemTypeName.Equals($"IdItemType_{typeof(T).Name}")).ToList().AsReadOnly();

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
        public async IAsyncEnumerable<T> GetItemsAsync<T>() where T : class, IIdItem
        {
            var iterator = Container.GetItemLinqQueryable<T>().Where(x => x.IdItemTypeName.Equals($"IdItemType_{typeof(T).Name}")).ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
        public IDataStoreQueryable<T> Query<T>() where T : class, IIdItem
            => new CosmosDataStoreQueryable<T>(Container, Container.GetItemLinqQueryable<T>().Where(x => x.IdItemTypeName.Equals($"IdItemType_{typeof(T).Name}")));

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
        public bool RemoveItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var result = Container.DeleteItemAsync<T>(id, new PartitionKey(id)).GetAwaiter().GetResult();
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
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
        public bool RemoveItem<T>(string? id, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var result = Container.DeleteItemAsync<T>(id, partitionKey).GetAwaiter().GetResult();
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
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
        public bool RemoveItem<T>(string? id, string? partitionKey) where T : class, IIdItem
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
        /// <typeparam name="T">The type of items to remove.</typeparam>
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
        public bool RemoveItem<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = Container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Id)).GetAwaiter().GetResult();
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
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
        public async Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var result = await Container.DeleteItemAsync<T>(id, new PartitionKey(id)).ConfigureAwait(false);
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
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
        public async Task<bool> RemoveItemAsync<T>(string? id, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var result = await Container.DeleteItemAsync<T>(id, partitionKey).ConfigureAwait(false);
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
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
        public Task<bool> RemoveItemAsync<T>(string? id, string? partitionKey) where T : class, IIdItem
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
        /// <typeparam name="T">The type of items to remove.</typeparam>
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
        public async Task<bool> RemoveItemAsync<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = await Container.DeleteItemAsync<T>(item.Id, new PartitionKey(item.Id)).ConfigureAwait(false);
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="item">
        /// <para>
        /// The item to remove.
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
        /// <remarks>
        /// This method is not implemented in the Cosmos implementation. Use the overload which
        /// takes an ID instead.
        /// </remarks>
        public async Task<bool> RemoveItemAsync<T>(T? item, PartitionKey partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = await Container.DeleteItemAsync<T>(item.Id, partitionKey).ConfigureAwait(false);
            return (int)result.StatusCode >= 200 && (int)result.StatusCode < 300;
        }

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="item">
        /// <para>
        /// The item to remove.
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
        /// <remarks>
        /// This method is not implemented in the Cosmos implementation. Use the overload which
        /// takes an ID instead.
        /// </remarks>
        public Task<bool> RemoveItemAsync<T>(T? item, string? partitionKey) where T : class, IIdItem
        {
            if (item is null)
            {
                return Task.FromResult(true);
            }
            if (string.IsNullOrEmpty(partitionKey))
            {
                return RemoveItemAsync<T>(item.Id);
            }
            return RemoveItemAsync<T>(item.Id, new PartitionKey(partitionKey));
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = Container.UpsertItemAsync(item).GetAwaiter().GetResult();
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item, PartitionKey partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = Container.UpsertItemAsync(item, partitionKey).GetAwaiter().GetResult();
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public bool StoreItem<T>(T? item, string? partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).GetAwaiter().GetResult();
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = await Container.UpsertItemAsync(item).ConfigureAwait(false);
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item, PartitionKey partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = await Container.UpsertItemAsync(item, partitionKey).ConfigureAwait(false);
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <param name="item">The item to store.</param>
        /// <param name="partitionKey">The partition key for items in the container.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        public async Task<bool> StoreItemAsync<T>(T? item, string? partitionKey, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            var result = await Container.UpsertItemAsync(item, new PartitionKey(partitionKey)).ConfigureAwait(false);
            if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 300)
            {
                _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);
                return true;
            }
            return false;
        }
    }
}
