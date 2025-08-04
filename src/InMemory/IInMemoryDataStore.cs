using System.Collections;

namespace Tavenem.DataStorage.InMemory;

/// <summary>
/// An in-memory data store.
/// </summary>
public interface IInMemoryDataStore : IDataStore
{
    /// <summary>
    /// The stored data.
    /// </summary>
    IDictionary Data { get; }
}
