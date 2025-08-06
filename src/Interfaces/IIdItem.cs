using System.Text.Json.Serialization;

namespace Tavenem.DataStorage;

/// <summary>
/// An item with an ID.
/// </summary>
/// <remarks>
/// <para>
/// This is the underlying interface for all implementations of <see cref="IIdItem{TSelf}"/>.
/// </para>
/// <para>
/// It uses a <see cref="string"/> key which may or may not be unique, depending on your persistence
/// requirements.
/// </para>
/// </remarks>
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(IdItem), IdItem.IIdItemTypeName)]
public interface IIdItem : IEquatable<IIdItem>
{
    /// <summary>
    /// The property name of the <see cref="IIdItem"/> type discriminator.
    /// </summary>
    public const string IdItemTypePropertyName = "_id_t";

    /// <summary>
    /// The type discriminator for this interface.
    /// </summary>
    public const string IIdItemTypeName = $":{nameof(IIdItem)}:";

    /// <summary>
    /// The ID of this item.
    /// </summary>
    [JsonPropertyName("id"), JsonInclude, JsonPropertyOrder(-2)]
    string Id { get; }

    /// <summary>
    /// A built-in, read-only type discriminator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inheritance and polymorphism are modeled by chaining subtypes with the ':' character as a
    /// separator.
    /// </para>
    /// <para>
    /// For example: ":BaseType:ChildType:".
    /// </para>
    /// <para>
    /// This property has a public <c>init</c> accessor for source generated deserialization
    /// support, but it is expected to do nothing.
    /// </para>
    /// <para>
    /// Note that this property is expected to always return the same value as <see
    /// cref="IIdItem.GetIdItemTypeName"/> for all instances of a given type implementing this
    /// interface.
    /// </para>
    /// </remarks>
    [JsonPropertyName(IdItemTypePropertyName), JsonInclude, JsonPropertyOrder(-1)]
    string IdItemTypeName { get; init; }

    /// <summary>
    /// Determines whether the specified <see cref="IIdItem"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IIdItem"/> instance to compare with this one.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="IIdItem"/> instance is equal to this
    /// once; otherwise, <see langword="false"/>.
    /// </returns>
    bool IEquatable<IIdItem>.Equals(IIdItem? other)
        => !string.IsNullOrEmpty(Id) && string.Equals(Id, other?.Id, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current object;
    /// otherwise, <see langword="false"/>.</returns>
    public virtual bool Equals(object? obj) => obj is IIdItem other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public virtual int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// <para>
    /// Gets the type discriminator for any instance of this interface as a static method.
    /// </para>
    /// <para>
    /// This method's default implementation is suitable only for the <see cref="IIdItem"/>
    /// interface itself. It should be overridden in implementations to return the correct
    /// discriminator value.
    /// </para>
    /// </summary>
    /// <returns>The type discriminator for any instance of this interface.</returns>
    /// <remarks>
    /// <para>
    /// Expected to start and end with the ':' character.
    /// </para>
    /// <para>
    /// Inheritance and polymorphism should be modeled by chaining subtypes with the ':' character
    /// as a separator.
    /// </para>
    /// <para>
    /// For example: ":BaseType:ChildType:".
    /// </para>
    /// <para>
    /// Note that the <see cref="IdItemTypeName"/> property for all instances of a given type
    /// implementing this interface are expected to always return the same value as this method.
    /// </para>
    /// </remarks>
    static virtual string GetIdItemTypeName() => IIdItemTypeName;

    /// <summary>
    /// Determines whether the specified <see cref="IIdItem"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IIdItem"/> instance to compare with this one.</param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="IIdItem"/> instance is equal to this
    /// once; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IdItemIsEqual(IIdItem? other)
        => ((IEquatable<IIdItem>)this).Equals(other);

    /// <summary>Returns a string equivalent of this instance.</summary>
    /// <returns>A string equivalent of this instance.</returns>
    public virtual string ToString() => Id;
}
