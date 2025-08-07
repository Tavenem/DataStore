namespace Tavenem.DataStorage;

/// <summary>
/// Allows <see cref="IIdItem"/> instances to be fetched, queried, removed, and stored.
/// </summary>
public interface IIdItemDataStore : IDataStore<string, IIdItem>
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
    /// <para>
    /// Whether the id is guaranteed to be unique or not depends on your persistence model and
    /// choice of implementation.
    /// </para>
    /// <para>
    /// The default implementation generates a new <see cref="Guid"/> and returns the result of its
    /// <see cref="Guid.ToString()"/> method.
    /// </para>
    /// </remarks>
    string IDataStore<string, IIdItem>.CreateNewIdFor<T>() => Guid.NewGuid().ToString();

    /// <summary>
    /// Creates a new id for an item of the given type.
    /// </summary>
    /// <param name="type">
    /// The type for which to generate an id.
    /// </param>
    /// <returns>
    /// A new id for an item of the given type; or <see langword="null"/> if the data
    /// source cannot generate primary keys.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Whether the id is guaranteed to be unique or not depends on your persistence model and
    /// choice of implementation.
    /// </para>
    /// <para>
    /// The default implementation generates a new <see cref="Guid"/> and returns the result of its
    /// <see cref="Guid.ToString()"/> method.
    /// </para>
    /// </remarks>
    string IDataStore<string, IIdItem>.CreateNewIdFor(Type type) => Guid.NewGuid().ToString();

    /// <inheritdoc />
    string IDataStore<string, IIdItem>.GetKey<T>(T item) => item.Id;

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose discriminator property is being obtained.</param>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.IdItemTypePropertyName"/> for <see cref="IIdItemDataStore"/>.
    /// </remarks>
    string? IDataStore<IIdItem>.GetTypeDiscriminatorName<T>(T item) => nameof(IIdItem.IdItemTypePropertyName);

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose type discriminator is being obtained.</param>
    /// <returns>
    /// The value of <paramref name="item"/>'s type discriminator, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.GetIdItemTypeName"/> for <see cref="IIdItemDataStore"/>.
    /// </remarks>
    string? IDataStore<IIdItem>.GetTypeDiscriminatorValue<T>(T item) => T.GetIdItemTypeName();

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
    ValueTask<bool> IDataStore<IIdItem>.RemoveItemAsync<T>(T? item, CancellationToken cancellationToken) where T : default
        => RemoveItemAsync(item?.Id, cancellationToken);
}
