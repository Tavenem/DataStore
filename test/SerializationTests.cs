using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json;
using Tavenem.DataStorage;

namespace Tavenem.DataStore.Test
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void PagedListTest()
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

        [TestMethod]
        public void PagedListDTOTest()
        {
            var value = new PagedList<int>(Enumerable.Range(0, 10), 1, 10, 10);
            var dto = new PagedListDTO<int>(value);
            var json = JsonSerializer.Serialize(dto);
            Console.WriteLine(json);
            var result = JsonSerializer.Deserialize<PagedListDTO<int>>(json);
            Assert.IsNotNull(result);
            var obj = result.ToPagedList();
            Assert.AreEqual(1, obj!.PageNumber);
            Assert.AreEqual(10, obj!.PageSize);
            Assert.AreEqual(10, obj!.TotalCount);
            Assert.AreEqual(10, obj!.Count);
            Assert.AreEqual(0, obj![0]);
            Assert.AreEqual(9, obj![9]);
            Assert.AreEqual(json, JsonSerializer.Serialize(new PagedListDTO<int>(obj)));
        }
    }
}
