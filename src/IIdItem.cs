namespace Tavenem.DataStorage;

/// <summary>
/// An item with an ID.
/// </summary>
/// <remarks>
/// <para>
/// This is the basic unit of persistence for implementations of <see cref="IDataStore"/>.
/// </para>
/// <para>
/// It uses a <see cref="string"/> key which may or may not be unique, depending on your
/// persistence requirements.
/// </para>
/// </remarks>
public interface IIdItem : IEquatable<IIdItem>
{
    /// <summary>
    /// The ID of this item.
    /// </summary>
    string Id { get; }
}
