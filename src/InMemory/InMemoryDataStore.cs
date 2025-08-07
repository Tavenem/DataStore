namespace Tavenem.DataStorage.InMemory;

/// <summary>
/// An in-memory data store for <see cref="IIdItem"/> instances.
/// </summary>
public class InMemoryDataStore : InMemoryDataStore<string, IIdItem>, IIdItemDataStore
{
    /// <inheritdoc />
    public override string? CreateNewIdFor<T>() => Guid.NewGuid().ToString();

    /// <inheritdoc />
    public override string? CreateNewIdFor(Type type) => Guid.NewGuid().ToString();

    /// <inheritdoc />
    public override string GetKey<T>(T item) => item.Id;

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.IdItemTypePropertyName"/> for <see cref="InMemoryDataStore"/>.
    /// </remarks>
    public override string? GetTypeDiscriminatorName<T>() => nameof(IIdItem.IdItemTypePropertyName);

    /// <summary>
    /// Gets the name of the property used to discriminate types, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose discriminator property is being obtained.</param>
    /// <returns>
    /// The name of the property used to discriminate types, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.IdItemTypePropertyName"/> for <see cref="InMemoryDataStore"/>.
    /// </remarks>
    public override string? GetTypeDiscriminatorName<T>(T item) => GetTypeDiscriminatorName<T>();

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <returns>
    /// The value of <typeparamref name="T"/>'s type discriminator, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.GetIdItemTypeName"/> for <see cref="InMemoryDataStore"/>.
    /// </remarks>
    public override string? GetTypeDiscriminatorValue<T>() => T.GetIdItemTypeName();

    /// <summary>
    /// Gets the value of the item's type discriminator, if any.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The item whose type discriminator is being obtained.</param>
    /// <returns>
    /// The value of <paramref name="item"/>'s type discriminator, if any.
    /// </returns>
    /// <remarks>
    /// Always returns <see cref="IIdItem.GetIdItemTypeName"/> for <see cref="InMemoryDataStore"/>.
    /// </remarks>
    public override string? GetTypeDiscriminatorValue<T>(T item) => GetTypeDiscriminatorValue<T>();
}
