﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverFoundry.DataStore
{
    /// <summary>
    /// Static methods for storing and retrieving <see cref="IdItem"/> instances.
    /// </summary>
    public static class DataStore
    {
        /// <summary>
        /// <para>
        /// The current underlying data store.
        /// </para>
        /// <para>
        /// Can be set to any implementation of <see cref="IDataStore"/>.
        /// </para>
        /// <para>
        /// By default, starts set to an instance of <see cref="InMemoryDataStore"/>.
        /// </para>
        /// </summary>
        public static IDataStore Instance = new InMemoryDataStore();

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// Whether the id is guaranteed to be unique or not depends on your persistence model and
        /// choice of implementation.
        /// </para>
        /// <para>
        /// The default <see cref="InMemoryDataStore"/> implementation generates a new <see
        /// cref="Guid"/> and returns the result of its <see cref="Guid.ToString()"/> method.
        /// </para>
        /// </remarks>
        public static string CreateNewIdFor<T>() where T : class, IIdItem => Instance.CreateNewIdFor<T>();

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
        public static string CreateNewIdFor(Type type) => Instance.CreateNewIdFor(type);

        /// <summary>
        /// Gets the <see cref="IdItem"/> with the given <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IdItem"/> to retrieve.</typeparam>
        /// <param name="id">The unique id of the item to retrieve.</param>
        /// <returns>The item with the given id, or <see langword="null"/> if no item was found with
        /// that id.</returns>
        public static Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem
            => Instance.GetItemAsync<T>(id);

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public static Task<IReadOnlyList<T>> GetItemsAsync<T>() where T : class, IIdItem
            => Instance.GetItemsAsync<T>();

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public static Task<IReadOnlyList<T>> GetItemsWhereAsync<T>(Func<T, bool> condition) where T : class, IIdItem
            => Instance.GetItemsWhereAsync(condition);

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public static Task<IReadOnlyList<T>> GetItemsWhereAwaitAsync<T>(Func<T, Task<bool>> condition) where T : class, IIdItem
            => Instance.GetItemsWhereAwaitAsync(condition);

        /// <summary>
        /// Forms a query for items in the data source of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to query.</typeparam>
        /// <returns>An <see cref="IQueryable{T}"/> of items in the data store of the given
        /// type.</returns>
        /// <remarks>
        /// Knowledge of the underlying persistence model is more important when using this method.
        /// </remarks>
        public static IQueryable<T> Query<T>() where T : class, IIdItem => Instance.Query<T>();

        /// <summary>
        /// Removes the stored item with the given id.
        /// </summary>
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
        public static Task<bool> RemoveItemAsync(string? id)
            => Instance.RemoveItemAsync(id);

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
        public static Task<bool> StoreItemAsync<T>(T item) where T : class, IIdItem
            => Instance.StoreItemAsync(item);
    }
}
