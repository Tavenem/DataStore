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
}
