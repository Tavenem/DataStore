using System;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// Allows <see cref="IIdItem"/> instances to be stored and retrieved.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// Whether the id is guaranteed to be unique or not depends on your persistence model and
        /// choice of implementation.
        /// </remarks>
        string CreateNewIdFor<T>() where T : class, IIdItem;

        /// <summary>
        /// Creates a new id for an item of the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type for which to generate an id. Expected to be an instance of
        /// <see cref="IIdItem"/>, but should not throw an exception even if a different type is
        /// supplied.</param>
        /// <returns>A new id for an item of the given <paramref name="type"/>.</returns>
        /// <remarks>
        /// Whether the id is guaranteed to be unique or not depends on your persistence model and
        /// choice of implementation.
        /// </remarks>
        string CreateNewIdFor(Type type);

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        T? GetItem<T>(string? id) where T : class, IIdItem;

        /// <summary>
        /// Gets the <see cref="IIdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>
        /// The item with the given id, or <see langword="null"/> if no item was found with that id.
        /// </returns>
        /// <remarks>
        /// This presumes that <paramref name="id"/> is a unique key, and therefore returns only one
        /// result. If your persistence model allows for non-unique keys and multiple results, use
        /// an appropriately formed <see cref="Query{T}"/>.
        /// </remarks>
        Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem;

        /// <summary>
        /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
        IDataStoreQueryable<T> Query<T>() where T : class, IIdItem;

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        bool RemoveItem<T>(string? id) where T : class, IIdItem;

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <param name="item">
        /// <para>
        /// The item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        bool RemoveItem(IIdItem? item);

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <typeparam name="T">The type of items to remove.</typeparam>
        /// <param name="id">
        /// <para>
        /// The id of the item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem;

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
        /// <param name="item">
        /// <para>
        /// The item to remove.
        /// </para>
        /// <para>
        /// If <see langword="null"/> or empty no operation takes place, and <see langword="true"/>
        /// is returned to indicate that there was no failure.
        /// </para>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the item was successfully removed; otherwise <see
        /// langword="false"/>.
        /// </returns>
        Task<bool> RemoveItemAsync(IIdItem? item);

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        bool StoreItem<T>(T? item) where T : class, IIdItem;

        /// <summary>
        /// Upserts the given <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> to upsert.</typeparam>
        /// <returns>
        /// <see langword="true"/> if the item was successfully persisted to the data store;
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// If the item is <see langword="null"/>, does nothing and returns <see langword="true"/>,
        /// to indicate that the operation did not fail (even though no storage operation took
        /// place, neither did any failure).
        /// </remarks>
        Task<bool> StoreItemAsync<T>(T? item) where T : class, IIdItem;
    }
}
