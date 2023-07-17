using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json;
using Tavenem.DataStorage;

namespace Tavenem.DataStore.Test;

[TestClass]
public partial class SerializationTests
{
    [TestMethod]
    public void IdItemTest()
    {
        var value = new TestIdItem("test") { TestProperty = 1 };
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
        Assert.AreEqual(10, result!.Count);
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
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result!.PageNumber);
        Assert.AreEqual(10, result!.PageSize);
        Assert.AreEqual(10, result!.TotalCount);
        Assert.AreEqual(10, result!.Count);
        Assert.AreEqual("0", result![0].Id);
        Assert.AreEqual("9", result![9].Id);
        Assert.AreEqual(json, JsonSerializer.Serialize(result2, options));
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
        Assert.AreEqual(10, result!.Count);
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
        Assert.AreEqual(10, result!.Count);
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
        Assert.AreEqual(10, result!.Count);
        Assert.AreEqual(0, result![0]);
        Assert.AreEqual(9, result![9]);
        Assert.AreEqual(json, JsonSerializer.Serialize(result));
    }
}
