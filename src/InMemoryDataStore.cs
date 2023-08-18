namespace Tavenem.DataStorage;

/// <summary>
/// An in-memory data store for <see cref="IIdItem"/> instances.
/// </summary>
public class InMemoryDataStore : IDataStore
{
    private Dictionary<string, IIdItem> _data = new();
    /// <summary>
    /// The stored data.
    /// </summary>
    public Dictionary<string, IIdItem> Data
    {
        get => _data;
        set => _data = value ?? new();
    }

    /// <summary>
    /// <para>
    /// Sets the default period after which cached items are considered stale.
    /// </para>
    /// <para>
    /// This is left at the default value for <see cref="InMemoryDataStore"/>, which does not
    /// support caching.
    /// </para>
    /// </summary>
    public TimeSpan DefaultCacheTimeout { get; set; }

    /// <summary>
    /// <para>
    /// Indicates whether this <see cref="IDataStore"/> implementation allows items to be
    /// cached.
    /// </para>
    /// <para>
    /// This is <see langword="false"/> for <see cref="InMemoryDataStore"/>.
    /// </para>
    /// </summary>
    public bool SupportsCaching => false;

    /// <summary>
    /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
    /// id.</typeparam>
    /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
    /// type.</returns>
    /// <remarks>
    /// <para>
    /// The <see cref="InMemoryDataStore"/> implementation generates a new <see cref="Guid"/>
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
        else if (Data.TryGetValue(id, out var item)
            && item is T t)
        {
            return t;
        }
        return default;
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
    public ValueTask<T?> GetItemAsync<T>(string? id, TimeSpan? cacheTimeout = null) where T : class, IIdItem => new(GetItem<T>(id));

    /// <summary>
    /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
    /// </summary>
    /// <typeparam name="T">The type of item to query.</typeparam>
    /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
    public IDataStoreQueryable<T> Query<T>() where T : class, IIdItem => new InMemoryDataStoreQueryable<T>(Data.Values.OfType<T>());

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
    public bool RemoveItem<T>(string? id) where T : class, IIdItem => string.IsNullOrEmpty(id) || Data.Remove(id);

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
    public bool RemoveItem<T>(T? item) where T : class, IIdItem => item is null || Data.Remove(item.Id);

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
    public Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem => Task.FromResult(RemoveItem<T>(id));

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
    public Task<bool> RemoveItemAsync<T>(T? item) where T : class, IIdItem => Task.FromResult(RemoveItem(item));

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
        Data[item.Id] = item;
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
    public Task<bool> StoreItemAsync<T>(T? item, TimeSpan? cacheTimeout = null) where T : class, IIdItem => Task.FromResult(StoreItem(item));
}
