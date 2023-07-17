using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tavenem.DataStorage;

namespace Tavenem.DataStore.Test;

public partial class SerializationTests
{
    [JsonSerializable(typeof(IIdItem))]
    [JsonSerializable(typeof(TestIdItem))]
    [JsonSerializable(typeof(IReadOnlyList<IIdItem>))]
    [JsonSerializable(typeof(IReadOnlyList<TestIdItem>))]
    [JsonSerializable(typeof(PagedList<IIdItem>))]
    [JsonSerializable(typeof(PagedList<TestIdItem>))]
    public partial class TestSourceGenerationContext
        : JsonSerializerContext
    { }
}
