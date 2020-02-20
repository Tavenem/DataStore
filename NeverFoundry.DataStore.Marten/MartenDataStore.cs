﻿using Marten;
using Marten.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.Marten
{
    /// <summary>
    /// A data store for <see cref="IIdItem"/> instances backed by a Marten implementation of
    /// PostgreSQL.
    /// </summary>
    public class MartenDataStore : IDataStore
    {
        /// <summary>
        /// The <see cref="IDocumentStore"/> used for all transactions.
        /// </summary>
        public IDocumentStore DocumentStore { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="MartenDataStore"/>.
        /// </summary>
        /// <param name="documentStore">The <see cref="IDocumentStore"/> used for all transactions.</param>
        public MartenDataStore(IDocumentStore documentStore) => DocumentStore = documentStore;

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="MartenDataStore"/> implementation generates a new <see cref="Guid"/>
        /// and returns the result of its <see cref="Guid.ToString()"/> method.
        /// </para>
        /// </remarks>
        public string CreateNewIdFor<T>() where T : IIdItem => Guid.NewGuid().ToString();

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
        public string CreateNewIdFor(Type type) => Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemWhere<T>(Func<T, bool> condition) where T : class, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().FirstOrDefault(condition);
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereAsync<T>(Func<T, bool> condition) where T : class, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            return await session.Query<T>().Where(condition).AsQueryable().FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereAwaitAsync<T>(Func<T, ValueTask<bool>> condition) where T : class, IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                list = await session.Query<T>().ToListAsync().ConfigureAwait(false);
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemWhereOrderedBy<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : class, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (descending)
            {
                return session.Query<T>().Where(condition).OrderByDescending(selector).FirstOrDefault();
            }
            else
            {
                return session.Query<T>().Where(condition).OrderBy(selector).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereOrderedByAsync<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : class, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (descending)
            {
                return await session.Query<T>().Where(condition).OrderByDescending(selector).AsQueryable().FirstOrDefaultAsync().ConfigureAwait(false);
            }
            else
            {
                return await session.Query<T>().Where(condition).OrderBy(selector).AsQueryable().FirstOrDefaultAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereOrderedByAwaitAsync<T, TKey>(Func<T, ValueTask<bool>> condition, Func<T, TKey> selector, bool descending = false) where T : class, IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                if (descending)
                {
                    list = await session.Query<T>().OrderByDescending(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    list = await session.Query<T>().OrderBy(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructWhere<T>(Func<T, bool> condition) where T : struct, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (session.Query<T>().Any(condition))
            {
                return session.Query<T>().First(condition);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereAsync<T>(Func<T, bool> condition) where T : struct, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (await session.Query<T>().Where(condition).AsQueryable().AnyAsync().ConfigureAwait(false))
            {
                return await session.Query<T>().Where(condition).AsQueryable().FirstAsync().ConfigureAwait(false);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereAwaitAsync<T>(Func<T, ValueTask<bool>> condition) where T : struct, IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                list = await session.Query<T>().ToListAsync().ConfigureAwait(false);
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructWhereOrderedBy<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : struct, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (!session.Query<T>().Any(condition))
            {
                return null;
            }
            if (descending)
            {
                return session.Query<T>().Where(condition).OrderByDescending(selector).First();
            }
            else
            {
                return session.Query<T>().Where(condition).OrderBy(selector).First();
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereOrderedByAsync<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : struct, IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (!await session.Query<T>().Where(condition).AsQueryable().AnyAsync().ConfigureAwait(false))
            {
                return null;
            }
            if (descending)
            {
                return await session.Query<T>().Where(condition).OrderByDescending(selector).AsQueryable().FirstAsync().ConfigureAwait(false);
            }
            else
            {
                return await session.Query<T>().Where(condition).OrderBy(selector).AsQueryable().FirstAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereOrderedByAwaitAsync<T, TKey>(Func<T, ValueTask<bool>> condition, Func<T, TKey> selector, bool descending = false) where T : struct, IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                if (descending)
                {
                    list = await session.Query<T>().OrderByDescending(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    list = await session.Query<T>().OrderBy(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    return item;
                }
            }
            return null;
        }

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
        public T? GetItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            using var session = DocumentStore.LightweightSession();
            return session.Load<T>(id);
        }

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
        public async Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            using var session = DocumentStore.LightweightSession();
            return await session.LoadAsync<T>(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItems<T>() where T : IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsAsync<T>() where T : IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                list = await session.Query<T>().ToListAsync().ConfigureAwait(false);
            }
            foreach (var item in list)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItemsWhere<T>(Func<T, bool> condition) where T : IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().Where(condition).ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereAsync<T>(Func<T, bool> condition) where T : IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                list = await session.Query<T>().Where(condition).AsQueryable().ToListAsync().ConfigureAwait(false);
            }
            foreach (var item in list)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereAwaitAsync<T>(Func<T, ValueTask<bool>> condition) where T : IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                list = await session.Query<T>().ToListAsync().ConfigureAwait(false);
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItemsWhereOrderedBy<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : IIdItem
        {
            using var session = DocumentStore.LightweightSession();
            if (descending)
            {
                return session.Query<T>().Where(condition).OrderByDescending(selector).ToList();
            }
            else
            {
                return session.Query<T>().Where(condition).OrderBy(selector).ToList();
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereOrderedByAsync<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, bool descending = false) where T : IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                if (descending)
                {
                    list = await session.Query<T>().Where(condition).OrderByDescending(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    list = await session.Query<T>().Where(condition).OrderBy(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
            }
            foreach (var item in list)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public async IAsyncEnumerable<T> GetItemsWhereOrderedByAwaitAsync<T, TKey>(Func<T, ValueTask<bool>> condition, Func<T, TKey> selector, bool descending = false) where T : IIdItem
        {
            IReadOnlyList<T> list;
            using (var session = DocumentStore.LightweightSession())
            {
                if (descending)
                {
                    list = await session.Query<T>().OrderByDescending(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    list = await session.Query<T>().OrderBy(selector).AsQueryable().ToListAsync().ConfigureAwait(false);
                }
            }
            foreach (var item in list)
            {
                if (await condition.Invoke(item).ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

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
        public IPagedList<T> GetPage<T>(int pageNumber, int pageSize)
        {
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().ToPagedList(pageNumber, pageSize).AsPagedList();
        }

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
        public async Task<IPagedList<T>> GetPageAsync<T>(int pageNumber, int pageSize)
        {
            using var session = DocumentStore.LightweightSession();
            return (await session.Query<T>().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList();
        }

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
        public IPagedList<T> GetPageOrderedBy<T, TKey>(Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false)
        {
            using var session = DocumentStore.LightweightSession();
            return descending
                ? session.Query<T>().OrderByDescending(selector).AsQueryable().ToPagedList(pageNumber, pageSize).AsPagedList()
                : session.Query<T>().OrderBy(selector).AsQueryable().ToPagedList(pageNumber, pageSize).AsPagedList();
        }

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
        public async Task<IPagedList<T>> GetPageOrderedByAsync<T, TKey>(Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false)
        {
            using var session = DocumentStore.LightweightSession();
            return descending
                ? (await session.Query<T>().OrderByDescending(selector).AsQueryable().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList()
                : (await session.Query<T>().OrderBy(selector).AsQueryable().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList();
        }

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
        public IPagedList<T> GetPageWhere<T>(Func<T, bool> condition, int pageNumber, int pageSize)
        {
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().Where(condition).AsQueryable().ToPagedList(pageNumber, pageSize).AsPagedList();
        }

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
        public async Task<IPagedList<T>> GetPageWhereAsync<T>(Func<T, bool> condition, int pageNumber, int pageSize)
        {
            using var session = DocumentStore.LightweightSession();
            return (await session.Query<T>().Where(condition).AsQueryable().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList();
        }

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
        public IPagedList<T> GetPageWhereOrderedBy<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false)
        {
            using var session = DocumentStore.LightweightSession();
            return descending
                ? session.Query<T>().Where(condition).OrderByDescending(selector).AsQueryable().ToPagedList(pageNumber, pageSize).AsPagedList()
                : session.Query<T>().Where(condition).OrderBy(selector).AsQueryable().ToPagedList(pageNumber, pageSize).AsPagedList();
        }

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
        public async Task<IPagedList<T>> GetPageWhereOrderedByAsync<T, TKey>(Func<T, bool> condition, Func<T, TKey> selector, int pageNumber, int pageSize, bool descending = false)
        {
            using var session = DocumentStore.LightweightSession();
            return descending
                ? (await session.Query<T>().Where(condition).OrderByDescending(selector).AsQueryable().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList()
                : (await session.Query<T>().Where(condition).OrderBy(selector).AsQueryable().ToPagedListAsync(pageNumber, pageSize).ConfigureAwait(false)).AsPagedList();
        }

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
        public T? GetStruct<T>(string? id) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            using var session = DocumentStore.LightweightSession();
            return session.Query<T>().Where(x => string.Equals(x.Id, id, StringComparison.Ordinal)).Any()
                ? session.Load<T>(id)
                : (T?)null;
        }

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
        public async Task<T?> GetStructAsync<T>(string? id) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            using var session = DocumentStore.LightweightSession();
            return await session.Query<T>().Where(x => string.Equals(x.Id, id, StringComparison.Ordinal)).AnyAsync().ConfigureAwait(false)
                ? await session.LoadAsync<T>(id).ConfigureAwait(false)
                : (T?)null;
        }

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
        public bool RemoveItem<T>(string? id) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete<T>(id);
            session.SaveChanges();
            return true;
        }

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
        public bool RemoveItem(IIdItem? item)
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete(item);
            session.SaveChanges();
            return true;
        }

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
        public async Task<bool> RemoveItemAsync<T>(string? id) where T : IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete<T>(id);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

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
        public async Task<bool> RemoveItemAsync(IIdItem? item)
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Delete(item);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

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
        public bool StoreItem<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            session.SaveChanges();
            return true;
        }

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
        public async Task<bool> StoreItemAsync<T>(T? item) where T : class, IIdItem
        {
            if (item is null)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }

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
        public bool StoreStruct<T>(T? item) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            session.SaveChanges();
            return true;
        }

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
        public async Task<bool> StoreStructAsync<T>(T? item) where T : struct, IIdItem
        {
            if (!item.HasValue)
            {
                return true;
            }
            using var session = DocumentStore.LightweightSession();
            session.Store(item);
            await session.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
