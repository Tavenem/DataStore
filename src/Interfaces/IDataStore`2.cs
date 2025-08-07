using System.Text.Json.Serialization.Metadata;

namespace Tavenem.DataStorage;

/// <summary>
/// Allows data with a particular type of primary key to be fetched, queried, removed, and stored.
/// </summary>
/// <typeparam name="TKey">The type of primary key for all stored items.</typeparam>
/// <typeparam name="TItem">A shared interface for all stored items.</typeparam>
public interface IDataStore<TKey, in TItem> : IDataStore<TItem>
    where TKey : notnull
    where TItem : notnull
{
    /// <summary>
    /// Creates a new primary key for an item of the given type.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item for which to generate an id.
    /// </typeparam>
    /// <returns>
    /// A new primary key for an item of the given type; or <see langword="null"/> if the data
    /// source cannot generate primary keys.
    /// </returns>
    /// <remarks>
    /// Whether the id is guaranteed to be unique or not depends on your persistence model and
    /// choice of implementation.
    /// </remarks>
    TKey? CreateNewIdFor<T>() where T : TItem;

    /// <summary>
    /// Creates a new id for an item of the given type.
    /// </summary>
    /// <param name="type">
    /// The type for which to generate an id.
    /// </param>
    /// <returns>
    /// A new id for an item of the given type; or <see langword="null"/> if the data source cannot
    /// generate primary keys.
    /// </returns>
    /// <remarks>
    /// Whether the id is guaranteed to be unique or not depends on your persistence model and
    /// choice of implementation.
    /// </remarks>
    TKey? CreateNewIdFor(Type type);

    /// <summary>
    /// Gets the item with the given <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique id of the item to retrieve.</param>
    /// <param name="cacheTimeout">
    /// If this item is cached, this value (if supplied) will override <see
    /// cref="IDataStore.DefaultCacheTimeout"/>.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The item with the given id, or <see langword="null"/> if no item was found with that id.
    /// </returns>
    /// <remarks>
    /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
    /// result. If your persistence model allows for non-unique keys and multiple results, use an
    /// appropriately formed <see cref="IDataStore{TItem}.Query{T}"/>.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<T?> GetItemAsync<T>(TKey? id, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem;

    /// <summary>
    /// Gets the item with the given <paramref name="id"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to retrieve.</typeparam>
    /// <param name="id">The unique id of the item to retrieve.</param>
    /// <param name="typeInfo">
    /// <para>
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="T"/>.
    /// </para>
    /// <para>
    /// This parameter is useful only for data sources which (de)serialize to/from JSON, but is
    /// provided so that callers without knowledge of the underlying storage implementation may
    /// supply the <see cref="JsonTypeInfo{T}"/> (when available) in case it might be necessary.
    /// When the parameter is unnecessary, the implementation should provide identical behavior to
    /// <see cref="GetItemAsync{T}(TKey?, TimeSpan?, CancellationToken)"/>.
    /// </para>
    /// </param>
    /// <param name="cacheTimeout">
    /// If this item is cached, this value (if supplied) will override <see
    /// cref="IDataStore.DefaultCacheTimeout"/>.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The item with the given id, or <see langword="null"/> if no item was found with that id.
    /// </returns>
    /// <remarks>
    /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
    /// result. If your persistence model allows for non-unique keys and multiple results, use an
    /// appropriately formed <see cref="IDataStore{TItem}.Query{T}"/>.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<T?> GetItemAsync<T>(TKey? id, JsonTypeInfo<T>? typeInfo, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem;

    /// <summary>
    /// Get the primary key of the given item.
    /// </summary>
    /// <typeparam name="T">The type of item for which to retrieve a key.</typeparam>
    /// <param name="item">The item whose key is to be retrieved.</param>
    /// <returns>The item's current primary key.</returns>
    TKey GetKey<T>(T item) where T : TItem;

    /// <summary>
    /// Removes the stored item with the given id.
    /// </summary>
    /// <param name="id">
    /// <para>
    /// The id of the item to remove.
    /// </para>
    /// <para>
    /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
    /// is returned to indicate that there was no failure.
    /// </para>
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the item was successfully removed; otherwise <see
    /// langword="false"/>.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<bool> RemoveItemAsync(TKey? id, CancellationToken cancellationToken = default);
}
