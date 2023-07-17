using System;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.DataStore.Test;

public class TestIdItem : IdItem
{
    public const string TestIdItemTypeName = ":TestIdItem:";

    public int TestProperty { get; set; }

    [JsonInclude, JsonPropertyOrder(-1)]
    public override string IdItemTypeName
    {
        get => TestIdItemTypeName;
        set { }
    }

    public TestIdItem() => Id = Guid.NewGuid().ToString();

    [JsonConstructor]
    public TestIdItem(string id) : base(id) { }
}
