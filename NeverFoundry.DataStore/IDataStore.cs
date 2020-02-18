using System;
using System.Collections.Generic;
using System.Linq;
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
        string CreateNewIdFor<T>() where T : IIdItem;

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
        /// <see cref="GetItemsWhere{T}(Func{T, bool})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        T? GetItem<T>(string? id) where T : class, IIdItem;

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
        /// <see cref="GetItemsWhereAsync{T}(Func{T, bool})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem;

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
        /// <see cref="GetItemsWhere{T}(Func{T, bool})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        T? GetStruct<T>(string? id) where T : struct, IIdItem;

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
        /// <see cref="GetItemsWhereAsync{T}(Func{T, bool})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        Task<T?> GetStructAsync<T>(string? id) where T : struct, IIdItem;

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        IReadOnlyList<T> GetItems<T>() where T : IIdItem;

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of items in the data store of the given
        /// type.</returns>
        IAsyncEnumerable<T> GetItemsAsync<T>() where T : IIdItem;

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        IReadOnlyList<T> GetItemsWhere<T>(Func<T, bool> condition) where T : IIdItem;

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of items in the data store of the given
        /// type.</returns>
        IAsyncEnumerable<T> GetItemsWhereAsync<T>(Func<T, bool> condition) where T : IIdItem;

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of items in the data store of the given
        /// type.</returns>
        IAsyncEnumerable<T> GetItemsWhereAwaitAsync<T>(Func<T, ValueTask<bool>> condition) where T : IIdItem;

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        IPagedList<T> GetPage<T>(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        Task<IPagedList<T>> GetPageAsync<T>(int pageNumber, int pageSize);

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        IPagedList<T> GetPageOrderedBy<T, TKey>(Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false);

        /// <summary>
        /// Gets a number of items in the data store of the given type equal to <paramref
        /// name="pageSize"/>, after skipping <paramref name="pageNumber"/> multiples of that
        /// amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        Task<IPagedList<T>> GetPageOrderedByAsync<T, TKey>(Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false);

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        IPagedList<T> GetPageWhere<T>(Func<T, bool> condition, int pageNumber, int pageSize);

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        Task<IPagedList<T>> GetPageWhereAsync<T>(Func<T, bool> condition, int pageNumber, int pageSize);

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        IPagedList<T> GetPageWhereOrderedBy<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false);

        /// <summary>
        /// Gets a number of items in the data store of the given type which satisfy the given
        /// condition equal to <paramref name="pageSize"/>, after skipping <paramref
        /// name="pageNumber"/> multiples of that amount, when sorted by the given criteria.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IPagedList{T}"/> of items in the data store of the given
        /// type.</returns>
        Task<IPagedList<T>> GetPageWhereOrderedByAsync<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false);

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
        bool RemoveItem<T>(string? id) where T : IIdItem;

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
        Task<bool> RemoveItemAsync<T>(string? id) where T : IIdItem;

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
        bool StoreStruct<T>(T? item) where T : struct, IIdItem;

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
        Task<bool> StoreStructAsync<T>(T? item) where T : struct, IIdItem;
    }
}
