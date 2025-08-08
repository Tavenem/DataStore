using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage;

/// <summary>
/// Allows data to be fetched, queried, removed, and stored.
/// </summary>
/// <typeparam name="TItem">A shared interface for all stored items.</typeparam>
public interface IDataStore<in TItem> : IDataStore where TItem : notnull
{
    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    /// <remarks>
    /// This method may return <see langword="null"/> unconditionally if the data source does not
    /// make use of type discriminators.
    /// </remarks>
    string? GetTypeDiscriminatorName<T>() where T : TItem => null;

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose discriminator property is being obtained.</param>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    /// <remarks>
    /// This method may return <see langword="null"/> unconditionally if the data source does not
    /// make use of type discriminators.
    /// </remarks>
    string? GetTypeDiscriminatorName<T>(T item) where T : TItem => null;

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The value of <typeparamref name="T"/>'s type discriminator, if any.
    /// </returns>
    /// <remarks>
    /// This method may return <see langword="null"/> unconditionally if the data source does not
    /// make use of type discriminators.
    /// </remarks>
    string? GetTypeDiscriminatorValue<T>() where T : TItem => null;

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose type discriminator is being obtained.</param>
    /// <returns>
    /// The value of <paramref name="item"/>'s type discriminator, if any.
    /// </returns>
    /// <remarks>
    /// This method may return <see langword="null"/> unconditionally if the data source does not
    /// make use of type discriminators.
    /// </remarks>
    string? GetTypeDiscriminatorValue<T>(T item) where T : TItem => null;

    /// <summary>
    /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
    /// </summary>
    /// <typeparam name="T">The type of item to query.</typeparam>
    /// <param name="typeInfo">
    /// <para>
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="T"/>.
    /// </para>
    /// <para>
    /// This parameter is useful only for data sources which (de)serialize to/from JSON, but the
    /// parameter is provided so that callers without knowledge of the underlying storage
    /// implementation may supply the <see cref="JsonTypeInfo{T}"/> (when available) in case it
    /// might be necessary.
    /// </para>
    /// </param>
    /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
    IDataStoreQueryable<T> Query<T>(JsonTypeInfo<T>? typeInfo = null) where T : notnull, TItem;

    /// <summary>
    /// Removes the stored item with the given id.
    /// </summary>
    /// <typeparam name="T">The type of item to remove.</typeparam>
    /// <param name="item">
    /// <para>
    /// The item to remove.
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
    ValueTask<bool> RemoveItemAsync<T>(T? item, CancellationToken cancellationToken = default) where T : TItem;

    /// <summary>
    /// Upserts the given <paramref name="item"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to upsert.</typeparam>
    /// <param name="item">The item to store.</param>
    /// <param name="cacheTimeout">
    /// If this item is cached, this value (if supplied) will override <see
    /// cref="IDataStore.DefaultCacheTimeout"/>.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The upserted item, if it was successfully persisted to the data store; otherwise <see
    /// langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Note that the returned object may be different from the provided <paramref name="item"/>. It
    /// may, for example, have had properties set by the data store (such as the primary key). This
    /// behavior depends on the implementation and the behavior of the data store.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<T?> StoreItemAsync<T>(T? item, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem;

    /// <summary>
    /// Upserts the given <paramref name="item"/>.
    /// </summary>
    /// <typeparam name="T">The type of item to upsert.</typeparam>
    /// <param name="item">The item to store.</param>
    /// <param name="typeInfo">
    /// <para>
    /// <see cref="JsonTypeInfo{T}"/> for <typeparamref name="T"/>.
    /// </para>
    /// <para>
    /// This parameter is useful only for data sources which (de)serialize to/from JSON, but the
    /// parameter is provided so that callers without knowledge of the underlying storage
    /// implementation may supply the <see cref="JsonTypeInfo{T}"/> (when available) in case it
    /// might be necessary. When the parameter is unnecessary, the implementation should provide
    /// identical behavior to <see cref="StoreItemAsync{T}(T, TimeSpan?, CancellationToken)"/>.
    /// </para>
    /// </param>
    /// <param name="cacheTimeout">
    /// If this item is cached, this value (if supplied) will override <see
    /// cref="IDataStore.DefaultCacheTimeout"/>.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// The upserted item, if it was successfully persisted to the data store; otherwise <see
    /// langword="null"/>.
    /// </returns>
    /// <remarks>
    /// Note that the returned object may be different from the provided <paramref name="item"/>. It
    /// may, for example, have had properties set by the data store (such as the primary key). This
    /// behavior depends on the implementation and the behavior of the data store.
    /// </remarks>
    /// <exception cref="OperationCanceledException">
    /// If the <see cref="CancellationToken"/> is cancelled.
    /// </exception>
    ValueTask<T?> StoreItemAsync<T>(T? item, JsonTypeInfo<T>? typeInfo, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem;
}
