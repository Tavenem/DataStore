using System.Text.Json.Serialization;

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
/// <para>
/// It also has a built-in, read-only type discriminator property.
/// </para>
/// </remarks>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(IdItem), nameof(IdItem))]
public interface IIdItem : IEquatable<IIdItem>
{
    /// <summary>
    /// The ID of this item.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// <para>
    /// A built-in, read-only type discriminator.
    /// </para>
    /// <para>
    /// This property has a default implementation in the <see cref="IIdItem"/> interface which
    /// uses reflection to generate a string with the format ":<c>GetType().Name</c>:".
    /// </para>
    /// <para>
    /// The property can (and should) be overridden in implementations to hard-code the
    /// discriminator value, both in order to avoid reflection and also to guard against
    /// potential breaking changes if a type is renamed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inheritance and polymorphism can be modeled by chaining subtypes with the ':' character
    /// as a separator.
    /// </para>
    /// <para>
    /// For example: ":BaseType:ChildType:".
    /// </para>
    /// </remarks>
    public string IdItemTypeName => $":{GetType().Name}:";
}
