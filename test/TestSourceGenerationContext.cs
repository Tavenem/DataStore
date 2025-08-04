using System.Text.Json.Serialization;
using Tavenem.DataStorage;
using Tavenem.DataStorage.InMemory;

namespace Tavenem.DataStore.Test;

public partial class SerializationTests
{
    [JsonSerializable(typeof(IIdItem))]
    [JsonSerializable(typeof(TestIdItem))]
    [JsonSerializable(typeof(IReadOnlyList<IIdItem>))]
    [JsonSerializable(typeof(IReadOnlyList<TestIdItem>))]
    [JsonSerializable(typeof(IPagedList<IIdItem>))]
    [JsonSerializable(typeof(PagedList<IIdItem>))]
    [JsonSerializable(typeof(PagedList<TestIdItem>))]
    [JsonSerializable(typeof(InMemoryDataStore))]
    public partial class TestSourceGenerationContext
        : JsonSerializerContext;
}
