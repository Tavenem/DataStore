using System.Text.Json;
using Tavenem.DataStorage;
using Tavenem.DataStorage.InMemory;

namespace Tavenem.DataStore.Test;

[TestClass]
public partial class SerializationTests(TestContext testContext)
{
    [TestMethod]
    public void IdItemTest()
    {
        var value = new TestIdItem();
        Assert.IsNotNull(value.Id);

        value = new TestIdItem("test") { TestProperty = 1 };
        Assert.AreEqual($":{nameof(TestIdItem)}:", value.IdItemTypeName);

        var json = JsonSerializer.Serialize(value);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<TestIdItem>(json);
        Assert.IsNotNull(result);
        Assert.AreEqual("test", result!.Id);
        Assert.AreEqual(1, result!.TestProperty);
        Assert.AreEqual(json, JsonSerializer.Serialize(result));

        IIdItem value2 = value;
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        json = JsonSerializer.Serialize(value2, options);
        Console.WriteLine(json);
        var result2 = JsonSerializer.Deserialize<IIdItem>(json, options);
        Assert.IsNotNull(result2);
        Assert.AreEqual("test", result2!.Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result2, options));
    }

    [TestMethod]
    public void IdItemSourceGeneratedTest()
    {
        var value = new TestIdItem("test") { TestProperty = 1 };
        var json = JsonSerializer.Serialize(value, TestSourceGenerationContext.Default.TestIdItem);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize(json, TestSourceGenerationContext.Default.TestIdItem);
        Assert.IsNotNull(result);
        Assert.AreEqual("test", result!.Id);
        Assert.AreEqual(1, result!.TestProperty);
        Assert.AreEqual(json, JsonSerializer.Serialize(result, TestSourceGenerationContext.Default.TestIdItem));

        IIdItem value2 = value;
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        options.TypeInfoResolverChain.Add(TestSourceGenerationContext.Default);
        json = JsonSerializer.Serialize(value2, options);
        Console.WriteLine(json);
        var result2 = JsonSerializer.Deserialize<IIdItem>(json, options);
        Assert.IsNotNull(result2);
        Assert.AreEqual("test", result2!.Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result2, options));
    }

    [TestMethod]
    public async Task InMemoryDataStoreTest()
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        var value = new InMemoryDataStore();

        var item = new TestIdItem("test") { TestProperty = 1 };
        Assert.AreEqual(IIdItem.IdItemTypePropertyName, value.GetTypeDiscriminatorName(item));
        Assert.AreEqual(TestIdItem.IIdItemTypeName, value.GetTypeDiscriminatorValue(item));

        Assert.IsNotNull(await value.StoreItemAsync(item, cancellationToken: testContext.CancellationTokenSource.Token));
        var json = JsonSerializer.Serialize(value, options);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<InMemoryDataStore>(json, options);
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Data);
        Assert.HasCount(1, result.Data);
        Assert.IsTrue(result.Data.ContainsKey("test"));
        Assert.AreEqual("test", result.Data["test"].Id);
        Assert.IsTrue(result.Data["test"] is TestIdItem);
        Assert.AreEqual(1, (result.Data["test"] as TestIdItem)?.TestProperty);
        Assert.AreEqual(json, JsonSerializer.Serialize(result, options));
    }

    [TestMethod]
    public async Task InMemoryDataStoreSourceGeneratedTest()
    {
        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        options.TypeInfoResolverChain.Add(TestSourceGenerationContext.Default);
        var value = new InMemoryDataStore();
        Assert.IsNotNull(await value.StoreItemAsync(new TestIdItem("test") { TestProperty = 1 }, cancellationToken: testContext.CancellationTokenSource.Token));
        var json = JsonSerializer.Serialize(value, options);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<InMemoryDataStore>(json, options);
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Data);
        Assert.HasCount(1, result.Data);
        Assert.IsTrue(result.Data.ContainsKey("test"));
        Assert.AreEqual("test", result.Data["test"].Id);
        Assert.IsTrue(result.Data["test"] is TestIdItem);
        Assert.AreEqual(1, (result.Data["test"] as TestIdItem)?.TestProperty);
        Assert.AreEqual(json, JsonSerializer.Serialize(result, options));
    }

    [TestMethod]
    public void PagedListIdItemTest()
    {
        var value = new PagedList<TestIdItem>(
            Enumerable
                .Range(0, 10)
                .Select(x => new TestIdItem(x.ToString()) { TestProperty = x }),
            1,
            10,
            10);
        var json = JsonSerializer.Serialize(value);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<PagedList<TestIdItem>>(json);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.PageNumber);
        Assert.AreEqual(10, result!.PageSize);
        Assert.AreEqual(10, result!.TotalCount);
        Assert.HasCount(10, result);
        Assert.AreEqual("0", result![0].Id);
        Assert.AreEqual(0, result![0].TestProperty);
        Assert.AreEqual("9", result![9].Id);
        Assert.AreEqual(9, result![9].TestProperty);
        Assert.AreEqual(json, JsonSerializer.Serialize(result));

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        var value2 = new PagedList<IIdItem>(
            Enumerable
                .Range(0, 10)
                .Select(x => new TestIdItem(x.ToString()) { TestProperty = x }),
            1,
            10,
            10);
        json = JsonSerializer.Serialize(value2, options);
        Console.WriteLine(json);
        var result2 = JsonSerializer.Deserialize<PagedList<IIdItem>>(json, options);
        Assert.IsNotNull(result2);
        Assert.AreEqual(1, result2!.PageNumber);
        Assert.AreEqual(10, result2!.PageSize);
        Assert.AreEqual(10, result2!.TotalCount);
        Assert.HasCount(10, result2);
        Assert.AreEqual("0", result2![0].Id);
        Assert.AreEqual("9", result2![9].Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result2, options));

        IPagedList<IIdItem> value3 = value2;
        json = JsonSerializer.Serialize(value3, options);
        Console.WriteLine(json);
        var result3 = JsonSerializer.Deserialize<IPagedList<IIdItem>>(json, options);
        Assert.IsNotNull(result3);
        Assert.AreEqual(1, result3!.PageNumber);
        Assert.AreEqual(10, result3!.PageSize);
        Assert.AreEqual(10, result3!.TotalCount);
        Assert.HasCount(10, result3);
        Assert.AreEqual("0", result3![0].Id);
        Assert.AreEqual("9", result3![9].Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result3, options));
    }

    [TestMethod]
    public void PagedListIdItemSourceGeneratedTest()
    {
        var value = new PagedList<TestIdItem>(
            Enumerable
                .Range(0, 10)
                .Select(x => new TestIdItem(x.ToString()) { TestProperty = x }),
            1,
            10,
            10);
        var json = JsonSerializer.Serialize(value, TestSourceGenerationContext.Default.PagedListTestIdItem);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize(json, TestSourceGenerationContext.Default.PagedListTestIdItem);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.PageNumber);
        Assert.AreEqual(10, result!.PageSize);
        Assert.AreEqual(10, result!.TotalCount);
        Assert.HasCount(10, result);
        Assert.AreEqual("0", result![0].Id);
        Assert.AreEqual(0, result![0].TestProperty);
        Assert.AreEqual("9", result![9].Id);
        Assert.AreEqual(9, result![9].TestProperty);
        Assert.AreEqual(
            json,
            JsonSerializer.Serialize(result, TestSourceGenerationContext.Default.PagedListTestIdItem));

        var options = new JsonSerializerOptions
        {
            TypeInfoResolver = new TestTypeResolver()
        };
        options.TypeInfoResolverChain.Add(TestSourceGenerationContext.Default);
        var value2 = new PagedList<IIdItem>(
            Enumerable
                .Range(0, 10)
                .Select(x => new TestIdItem(x.ToString()) { TestProperty = x }),
            1,
            10,
            10);
        json = JsonSerializer.Serialize(value2, options);
        Console.WriteLine(json);
        var result2 = JsonSerializer.Deserialize<PagedList<IIdItem>>(json, options);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.PageNumber);
        Assert.AreEqual(10, result!.PageSize);
        Assert.AreEqual(10, result!.TotalCount);
        Assert.HasCount(10, result);
        Assert.AreEqual("0", result![0].Id);
        Assert.AreEqual("9", result![9].Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result2, options));
    }

    [TestMethod]
    public void PagedListIntTest()
    {
        var value = new PagedList<int>(Enumerable.Range(0, 10), 1, 10, 10);
        var json = JsonSerializer.Serialize(value);
        Console.WriteLine(json);
        var result = JsonSerializer.Deserialize<PagedList<int>>(json);
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.PageNumber);
        Assert.AreEqual(10, result!.PageSize);
        Assert.AreEqual(10, result!.TotalCount);
        Assert.HasCount(10, result);
        Assert.AreEqual(0, result![0]);
        Assert.AreEqual(9, result![9]);
        Assert.AreEqual(json, JsonSerializer.Serialize(result));
    }
}
