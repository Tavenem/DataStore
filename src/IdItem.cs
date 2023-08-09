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
public abstract class IdItem : IIdItem, IEquatable<IdItem>
{
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
    /// <para>
    /// A built-in, read-only type discriminator.
    /// </para>
    /// <para>
    /// This property's default implementation uses reflection to generate a string with the format
    /// ":<c>GetType().Name</c>:".
    /// </para>
    /// <para>
    /// The property can (and should) be overridden in subclasses to hard-code the discriminator
    /// value, both in order to avoid reflection and also to guard against potential breaking
    /// changes if a type is renamed.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Inheritance and polymorphism can be modeled by chaining subtypes with the ':' character as a
    /// separator.
    /// </para>
    /// <para>
    /// For example: ":BaseType:ChildType:".
    /// </para>
    /// <para>
    /// The property's default implementation has a set accessor, but it performs no function. It
    /// exists to allow overriding implementations to provide both get and set accessors, if
    /// necessary.
    /// </para>
    /// </remarks>
    [JsonInclude, JsonPropertyOrder(-1)]
    public virtual string IdItemTypeName
    {
        get => $":{GetType().Name}:";
        set { }
    }

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
    /// Determines whether the specified <see cref="IIdItem"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IIdItem"/> instance to compare with this one.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IIdItem"/> instance is equal
    /// to this once; otherwise, <see langword="false"/>.</returns>
    public bool Equals(IIdItem? other)
        => !string.IsNullOrEmpty(Id) && string.Equals(Id, other?.Id, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the specified <see cref="IdItem"/> instance is equal to this one.
    /// </summary>
    /// <param name="other">The <see cref="IdItem"/> instance to compare with this one.</param>
    /// <returns><see langword="true"/> if the specified <see cref="IdItem"/> instance is equal
    /// to this once; otherwise, <see langword="false"/>.</returns>
    public bool Equals(IdItem? other)
        => !string.IsNullOrEmpty(Id) && string.Equals(Id, other?.Id, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><see langword="true"/> if the specified object is equal to the current object;
    /// otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj) => obj is IdItem other && Equals(other);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code for this instance.</returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>Returns a string equivalent of this instance.</summary>
    /// <returns>A string equivalent of this instance.</returns>
    public override string ToString() => Id;

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
