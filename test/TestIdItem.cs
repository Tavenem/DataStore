using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.DataStore.Test;

public class TestIdItem : IdItem, IIdItem<TestIdItem>
{
    /// <summary>
    /// The <see cref="IdItemTypeName"/> for this class.
    /// </summary>
    public new const string IIdItemTypeName = ":TestIdItem:";

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
    /// Note that this property is expected to always return the same value as <see
    /// cref="IIdItem{TSelf}.GetIdItemTypeName"/> for this type.
    /// </para>
    /// </remarks>
    [JsonPropertyName("_id_t"), JsonInclude, JsonPropertyOrder(-1)]
    public override string IdItemTypeName { get => IIdItemTypeName; init { } }

    public int TestProperty { get; set; }

    public TestIdItem() { }

    [JsonConstructor]
    public TestIdItem(string id) : base(id) { }

    /// <summary>
    /// Gets the <see cref="IdItemTypeName"/> for any instance of this class as a static method.
    /// </summary>
    /// <returns>The <see cref="IdItemTypeName"/> for any instance of this class.</returns>
    static string IIdItem.GetIdItemTypeName() => IIdItemTypeName;
}
