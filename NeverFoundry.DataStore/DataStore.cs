using System;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// Static methods for storing and retrieving <see cref="IdItem"/> instances.
    /// </summary>
    public static class DataStore
    {
        /// <summary>
        /// Sets the default period after which cached items are considered stale.
        /// </summary>
        public static TimeSpan DefaultCacheTimeout
        {
            get => Instance.DefaultCacheTimeout;
            set => Instance.DefaultCacheTimeout = value;
        }

        /// <summary>
        /// <para>
        /// The current underlying data store.
        /// </para>
        /// <para>
        /// Can be set to any implementation of <see cref="IDataStore"/>.
        /// </para>
        /// <para>
        /// By default, starts set to an instance of <see cref="InMemoryDataStore"/>.
        /// </para>
        /// </summary>
        public static IDataStore Instance = new InMemoryDataStore();

        /// <summary>
        /// Indicates whether this <see cref="IDataStore"/> implementation allows items to be
        /// cached.
        /// </summary>
        /// <remarks>
        /// This will usually be <see langword="true"/> for implementations which wrap persistent or
        /// networked storage systems, and <see langword="false"/> for in-memory implementations
        /// (which would not typically benefit from caching).
        /// </remarks>
        public static bool SupportsCaching => Instance.SupportsCaching;

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// Whether the id is guaranteed to be unique or not depends on your persistence model and
        /// choice of implementation.
        /// </para>
        /// <para>
        /// The default <see cref="InMemoryDataStore"/> implementation generates a new <see
        /// cref="Guid"/> and returns the result of its <see cref="Guid.ToString()"/> method.
        /// </para>
        /// </remarks>
        public static string CreateNewIdFor<T>() where T : class, IIdItem => Instance.CreateNewIdFor<T>();

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
        public static string CreateNewIdFor(Type type) => Instance.CreateNewIdFor(type);

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
        public static T? GetItem<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem => Instance.GetItem<T>(id, cacheTimeout);

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
        public static ValueTask<T?> GetItemAsync<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem
            => Instance.GetItemAsync<T>(id, cacheTimeout);

        /// <summary>
        /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
        public static IDataStoreQueryable<T> Query<T>() where T : class, IIdItem => Instance.Query<T>();

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
        public static bool RemoveItem<T>(string? id) where T : class, IIdItem => Instance.RemoveItem<T>(id);

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
        public static bool RemoveItem(IIdItem? item) => Instance.RemoveItem(item);

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
        public static Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem
            => Instance.RemoveItemAsync<T>(id);

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
        public static Task<bool> RemoveItemAsync(IIdItem? item)
            => Instance.RemoveItemAsync(item);

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
        public static bool StoreItem<T>(T? item, TimeSpan? cacheTimeout = null) where T : class, IIdItem => Instance.StoreItem(item, cacheTimeout);

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
        public static Task<bool> StoreItemAsync<T>(T? item, TimeSpan? cacheTimeout = null) where T : class, IIdItem
            => Instance.StoreItemAsync(item, cacheTimeout);
    }
}
