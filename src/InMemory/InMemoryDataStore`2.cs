using System.Collections;
using System.Numerics;
using System.Text.Json.Serialization.Metadata;
using Tavenem.DataStorage.Interfaces;

namespace Tavenem.DataStorage.InMemory;

/// <summary>
/// An in-memory data store for <typeparamref name="TItem"/> instances.
/// </summary>
/// <typeparam name="TKey">The type of primary key for all stored items.</typeparam>
/// <typeparam name="TItem">A shared interface for all stored items.</typeparam>
public abstract class InMemoryDataStore<TKey, TItem> : IInMemoryDataStore, IDataStore<TKey, TItem>
    where TKey : notnull
    where TItem : notnull
{
    /// <summary>
    /// The stored data.
    /// </summary>
    public Dictionary<TKey, TItem> Data
    {
        get;
        set => field = value ?? [];
    } = [];

    /// <inheritdoc />
    IDictionary IInMemoryDataStore.Data => Data;

    /// <summary>
    /// <para>
    /// Sets the default period after which cached items are considered stale.
    /// </para>
    /// <para>
    /// This is left at the default value for <see cref="InMemoryDataStore{TKey, TItem}"/>, which does not
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
    /// This is <see langword="false"/> for <see cref="InMemoryDataStore{TKey, TItem}"/>.
    /// </para>
    /// </summary>
    public bool SupportsCaching => false;

    /// <inheritdoc />
    public ValueTask<TSource> AverageAsync<TSource>(IDataStoreQueryable<TSource> source, CancellationToken cancellationToken = default) where TSource : INumberBase<TSource>
    {
        if (source is not InMemoryDataStoreQueryable<TSource> inMemoryQueryable)
        {
            throw new InvalidOperationException("The source IDataStoreQueryable is not a valid InMemoryDataStoreQueryable.");
        }

        try
        {
            var sum = TSource.Zero;
            var count = 0L;
            foreach (var item in inMemoryQueryable)
            {
                sum += item;
                count++;
            }
            return count == 0
                ? throw new InvalidOperationException("sequence contains no elements")
                : new(sum / TSource.CreateChecked(count));
        }
        catch (OverflowException) { }

        // overflow occurred, try less precise method which does not use a total
        try
        {
            var average = TSource.Zero;
            var count = inMemoryQueryable.LongCount();
            if (count == 0)
            {
                throw new InvalidOperationException("sequence contains no elements");
            }
            var tCount = TSource.CreateChecked(count);

            foreach (var item in inMemoryQueryable)
            {
                average += item / tCount;
            }

            return new(average);
        }
        catch (OverflowException)
        {
            throw new OverflowException("The average of the sequence is too large to be represented by the type.");
        }
    }

    /// <inheritdoc />
    public abstract TKey? CreateNewIdFor<T>() where T : TItem;

    /// <inheritdoc />
    public abstract TKey? CreateNewIdFor(Type type);

    /// <inheritdoc />
    public abstract TKey GetKey<T>(T item) where T : TItem;

    /// <inheritdoc />
    public ValueTask<T?> GetItemAsync<T>(TKey? id, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem
    {
        if (id is null)
        {
            return default;
        }
        else if (Data.TryGetValue(id, out var item)
            && item is T t)
        {
            return new(t);
        }
        return default;
    }

    /// <inheritdoc />
    public ValueTask<T?> GetItemAsync<T>(TKey? id, JsonTypeInfo<T>? typeInfo, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem
        => GetItemAsync<T>(id, cacheTimeout, cancellationToken);

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    public virtual string? GetTypeDiscriminatorName<T>() where T : TItem => null;

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose discriminator property is being obtained.</param>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    public virtual string? GetTypeDiscriminatorName<T>(T item) where T : TItem => null;

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The value of <typeparamref name="T"/>'s type discriminator, if any.
    /// </returns>
    public virtual string? GetTypeDiscriminatorValue<T>() where T : TItem => null;

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose type discriminator is being obtained.</param>
    /// <returns>
    /// The value of <paramref name="item"/>'s type discriminator, if any.
    /// </returns>
    public virtual string? GetTypeDiscriminatorValue<T>(T item) where T : TItem => null;

    /// <inheritdoc />
    public IDataStoreQueryable<T> Query<T>(JsonTypeInfo<T>? typeInfo = null) where T : TItem
        => new InMemoryDataStoreQueryable<T>(this, Data.Values.OfType<T>().AsQueryable());

    /// <inheritdoc />
    public ValueTask<bool> RemoveItemAsync<T>(TKey? id, CancellationToken cancellationToken = default) where T : TItem => new(id is null || Data.Remove(id));

    /// <inheritdoc />
    public ValueTask<bool> RemoveItemAsync<T>(T? item, CancellationToken cancellationToken = default) where T : TItem => new(item is null || Data.Remove(GetKey(item)));

    /// <inheritdoc />
    public ValueTask<T?> StoreItemAsync<T>(T? item, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem
    {
        if (item is null)
        {
            return new(item);
        }
        Data[GetKey(item)] = item;
        return new(item);
    }

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
    /// This parameter is ignored for <see cref="InMemoryDataStore{TKey, TItem}"/>; this method calls <see
    /// cref="StoreItemAsync{T}(T, TimeSpan?, CancellationToken)"/>.
    /// </para>
    /// </param>
    /// <param name="cacheTimeout">
    /// If this item is cached, this value (if supplied) will override <see
    /// cref="DefaultCacheTimeout"/>.
    /// </param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the item was successfully persisted to the data store; otherwise
    /// <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>, to
    /// indicate that the operation did not fail (even though no storage operation took place,
    /// neither did any failure).
    /// </para>
    /// </remarks>
    public ValueTask<T?> StoreItemAsync<T>(T? item, JsonTypeInfo<T>? typeInfo, TimeSpan? cacheTimeout = null, CancellationToken cancellationToken = default) where T : TItem
        => StoreItemAsync(item, cacheTimeout, cancellationToken);

    /// <inheritdoc />
    public ValueTask<TSource> SumAsync<TSource>(IDataStoreQueryable<TSource> source, CancellationToken cancellationToken = default) where TSource : INumberBase<TSource>
    {
        if (source is not InMemoryDataStoreQueryable<TSource> inMemoryQueryable)
        {
            throw new InvalidOperationException("The source IDataStoreQueryable is not a valid InMemoryDataStoreQueryable.");
        }

        var sum = TSource.Zero;
        foreach (var item in inMemoryQueryable)
        {
            sum += item;
        }
        return new(sum);
    }
}
