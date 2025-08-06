using System.Text.Json.Serialization;

namespace Tavenem.DataStorage;

/// <summary>
/// An item with an ID and a type discriminator.
/// </summary>
/// <remarks>
/// <para>
/// This is the basic unit of persistence for implementations of <see cref="IIdItemDataStore"/>.
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
[JsonDerivedType(typeof(IdItem), IdItem.IIdItemTypeName)]
public interface IIdItem<TSelf> : IIdItem, IEquatable<IIdItem<TSelf>> where TSelf : IIdItem<TSelf>
{
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
    [JsonPropertyName("_id_t"), JsonInclude, JsonPropertyOrder(-1)]
    string IIdItem.IdItemTypeName { get => TSelf.GetIdItemTypeName(); init { } }

    /// <summary>
    /// Determines whether the specified <see cref="IIdItem{TSelf}"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IIdItem{TSelf}"/> instance to compare with this one.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IIdItem{TSelf}"/> instance is equal
    /// to this once; otherwise, <see langword="false"/>.</returns>
    bool IEquatable<IIdItem<TSelf>>.Equals(IIdItem<TSelf>? other)
        => !string.IsNullOrEmpty(Id) && string.Equals(Id, other?.Id, StringComparison.Ordinal);

    /// <summary>
    /// Indicates whether two <see cref="IIdItem{TSelf}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static virtual bool operator ==(TSelf? left, TSelf? right) => left is null
        ? right is null
        : left.Equals(right);

    /// <summary>
    /// Indicates whether two <see cref="IIdItem{TSelf}"/> instances are unequal.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns>
    /// <see langword="true"/> if the instances are unequal; otherwise, <see langword="false"/>.
    /// </returns>
    public static virtual bool operator !=(TSelf? left, TSelf? right) => !(left == right);
}
