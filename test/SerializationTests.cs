using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tavenem.DataStorage;
using System;
using System.Linq;
using System.Text.Json;

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
    }
}
