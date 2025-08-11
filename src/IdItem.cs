using System.Text.Json.Serialization;

namespace Tavenem.DataStorage;

/// <summary>
/// An item with an ID.
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
/// The default, parameterless constructor automatically generates a new <see cref="Id"/> as a
/// random string based on a <see cref="Guid"/>.
/// </para>
/// <para>
/// Equality and hashing are performed with the <see cref="Id"/> alone, which presumes that Ids
/// are globally unique. If your persistence mechanism (and Id generation method) does not
/// require or produce unique keys, the equality and hash code generation methods should be
/// overridden in derived classes to ensure correct behavior.
/// </para>
/// </remarks>
public abstract class IdItem : IIdItem<IdItem>, IEquatable<IdItem>
{
    /// <summary>
    /// The <see cref="IdItemTypeName"/> for this class.
    /// </summary>
    public const string IIdItemTypeName = $":{nameof(IdItem)}:";

    /// <summary>
    /// <para>
    /// The ID of this item.
    /// </para>
    /// <para>
    /// Settable for initialization and serialization purposes, but is intended to be stable for
    /// existing items.
    /// </para>
    /// </summary>
    [JsonPropertyName("id"), JsonInclude, JsonPropertyOrder(-2)]
    public string Id { get; set; }

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
    /// cref="IIdItem.GetIdItemTypeName"/> for this type.
    /// </para>
    /// </remarks>
    [JsonPropertyName("_id_t"), JsonInclude, JsonPropertyOrder(-1)]
    public abstract string IdItemTypeName { get; init; }

    /// <summary>
    /// <para>
    /// Initializes a new instance of <see cref="IdItem"/>.
    /// </para>
    /// <para>
    /// Initializes <see cref="Id"/> to a random string based on a <see cref="Guid"/>.
    /// </para>
    /// </summary>
    protected IdItem() => Id = Guid.NewGuid().ToString();

    /// <summary>
    /// Initializes a new instance of <see cref="IdItem"/>.
    /// </summary>
    /// <param name="id">The item's <see cref="Id"/>.</param>
    protected IdItem(string id) => Id = id;

    /// <summary>
    /// <para>
    /// Gets the <see cref="IdItemTypeName"/> for any instance of this class as a static method.
    /// </para>
    /// <para>
    /// This method's default implementation is suitable only for the <see cref="IdItem"/> class
    /// itself. It should be overridden in subclasses to return the correct discriminator value.
    /// </para>
    /// </summary>
    /// <returns>The <see cref="IdItemTypeName"/> for any instance of this class.</returns>
    /// <remarks>
    /// <para>
    /// The value returned by this method is expected to start and end with the ':' character.
    /// </para>
    /// <para>
    /// Inheritance and polymorphism should be modeled by chaining subtypes with the ':' character
    /// as a separator.
    /// </para>
    /// <para>
    /// For example: ":BaseType:ChildType:".
    /// </para>
    /// <para>
    /// Note that the <see cref="IdItemTypeName"/> property for all instances of this type are
    /// expected to always return the same value as this method.
    /// </para>
    /// </remarks>
    static string IIdItem.GetIdItemTypeName() => IIdItemTypeName;

    /// <summary>
    /// Determines whether the specified <see cref="IIdItem{TSelf}"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">
    /// The <see cref="IIdItem{TSelf}"/> instance to compare with this one.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <see cref="IIdItem{TSelf}"/> instance is equal to
    /// this once; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(IIdItem? other) => ((IIdItem)this).IdItemIsEqual(other);

    /// <summary>
    /// Determines whether the specified <see cref="IdItem"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IdItem"/> instance to compare with this one.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IdItem"/> instance is equal
    /// to this once; otherwise, <see langword="false"/>.</returns>
    public bool Equals(IdItem? other) => ((IIdItem)this).IdItemIsEqual(other);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current object;
    /// otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj) => obj is IIdItem other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode() => ((IIdItem)this).GetHashCode();

    /// <summary>Returns a string equivalent of this instance.</summary>
    /// <returns>A string equivalent of this instance.</returns>
    public override string ToString() => ((IIdItem)this).ToString();

    /// <summary>
    /// Indicates whether two <see cref="IdItem"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true"/> if the instances are equal; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool operator ==(IdItem? left, IdItem? right) => left is null
        ? right is null
        : left.Equals(right);

    /// <summary>
    /// Indicates whether two <see cref="IdItem"/> instances are unequal.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true"/> if the instances are unequal; otherwise, <see
    /// langword="false"/>.</returns>
    public static bool operator !=(IdItem? left, IdItem? right) => !(left == right);
}
