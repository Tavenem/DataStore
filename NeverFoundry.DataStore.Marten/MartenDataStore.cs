using LazyCache;
using Marten;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.Marten
{
    /// <summary>
    /// A data store for <see cref="IIdItem"/> instances backed by a Marten implementation of
    /// PostgreSQL.
    /// </summary>
    public class MartenDataStore : IDataStore
    {
        private readonly IAppCache _cache = new CachingService();

        /// <summary>
        /// <para>
        /// Sets the default period after which cached items are considered stale.
        /// </para>
        /// <para>
        /// This defaults to ten minutes for <see cref="MartenDataStore"/>.
        /// </para>
        /// </summary>
        public TimeSpan DefaultCacheTimeout { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// The <see cref="IDocumentStore"/> used for all transactions.
        /// </summary>
        public IDocumentStore DocumentStore { get; set; }

        /// <summary>
        /// <para>
        /// Indicates whether this <see cref="IDataStore"/> implementation allows items to be
        /// cached.
        /// </para>
        /// <para>
        /// This is <see langword="true"/> for <see cref="MartenDataStore"/>.
        /// </para>
        /// </summary>
        public bool SupportsCaching => true;

        /// <summary>
        /// Initializes a new instance of <see cref="MartenDataStore"/>.
        /// </summary>
        /// <param name="documentStore">The <see cref="IDocumentStore"/> used for all transactions.</param>
        public MartenDataStore(IDocumentStore documentStore) => DocumentStore = documentStore;

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="MartenDataStore"/> implementation generates a new <see cref="Guid"/>
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
                () =>
                {
                    using var session = DocumentStore.LightweightSession();
                    return session.Load<T>(id);
                },
                cacheTimeout ?? DefaultCacheTimeout);
        }

        /// <summary>
        /// Gets the <see cref="IdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <param name="cacheTimeout">
        /// If this item is cached, this value (if supplied) will override <see cref="DefaultCacheTimeout"/>.
        /// </param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        public async ValueTask<T?> GetItemAsync<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default!;
            }
            return await _cache.GetOrAddAsync(
                id,
                () =>
                {
                    using var session = DocumentStore.LightweightSession();
                    return session.LoadAsync<T>(id);
                },
                cacheTimeout ?? DefaultCacheTimeout)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
        /// <remarks>
        /// This creates an <see cref="IQuerySession"/> which is disposed only when calling a method
        /// of <see cref="MartenDataStoreQueryable{T}"/> which does not itself return another
        /// instance of <see cref="MartenDataStoreQueryable{T}"/>. Ensure that you invoke such a
        /// method to properly dispose of the session.
        /// </remarks>
        public IDataStoreQueryable<T> Query<T>() where T : class, IIdItem
        {
            var session = DocumentStore.QuerySession();
            return new MartenDataStoreQueryable<T>(session, session.Query<T>());
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
        public bool RemoveItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete<T>(id);
            session.SaveChanges();
            return true;
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
        public bool RemoveItem(IIdItem? item)
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete(item);
            session.SaveChanges();
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
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        public async Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete<T>(id);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
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
        public async Task<bool> RemoveItemAsync(IIdItem? item)
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete(item);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
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
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            session.SaveChanges();

            _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);

            return true;
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
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            await session.SaveChangesAsync().ConfigureAwait(false);

            _cache.Add(item.Id, item, cacheTimeout ?? DefaultCacheTimeout);

            return true;
        }
    }
}
