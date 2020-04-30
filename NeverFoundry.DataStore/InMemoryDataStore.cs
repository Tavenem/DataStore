using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// An in-memory data store for <see cref="IIdItem"/> instances.
    /// </summary>
    public class InMemoryDataStore : IDataStore
    {
        private readonly Dictionary<string, IIdItem> _data = new Dictionary<string, IIdItem>();

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="InMemoryDataStore"/> implementation generates a new <see cref="Guid"/>
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
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
            => descending
            ? _data.Values.OfType<T>().OrderByDescending(selector.Compile()).FirstOrDefault()
            : _data.Values.OfType<T>().OrderBy(selector.Compile()).FirstOrDefault();

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public Task<T?> GetFirstItemOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
            => Task.FromResult(GetFirstItemOrderedBy(selector, descending));

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstItemWhere<T>(Expression<Func<T, bool>> condition) where T : class, IIdItem
            => _data.Values.OfType<T>().FirstOrDefault(condition.Compile());

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public Task<T?> GetFirstItemWhereAsync<T>(Expression<Func<T, bool>> condition) where T : class, IIdItem
            => Task.FromResult(GetFirstItemWhere(condition));

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstItemWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : class, IIdItem
            => await _data.Values.OfType<T>().ToAsyncEnumerable().FirstOrDefaultAwaitAsync(condition.Compile()).ConfigureAwait(false);

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
        public T? GetFirstItemWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
            => descending
            ? _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderByDescending(selector.Compile()).FirstOrDefault()
            : _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderBy(selector.Compile()).FirstOrDefault();

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
        public Task<T?> GetFirstItemWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
            => Task.FromResult(GetFirstItemWhereOrderedBy(condition, selector, descending));

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
        public async Task<T?> GetFirstItemWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : class, IIdItem
            => descending
            ? await _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderByDescending(selector.Compile()).FirstOrDefaultAsync().ConfigureAwait(false)
            : await _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderBy(selector.Compile()).FirstOrDefaultAsync().ConfigureAwait(false);

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            if (!_data.Values.OfType<T>().Any())
            {
                return null;
            }
            return descending
                ? _data.Values.OfType<T>().OrderByDescending(selector.Compile()).First()
                : _data.Values.OfType<T>().OrderBy(selector.Compile()).First();
        }

        /// <summary>
        /// Gets the first item in the data store of the given type, in the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public Task<T?> GetFirstStructOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
            => Task.FromResult(GetFirstStructOrderedBy(selector, descending));

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public T? GetFirstStructWhere<T>(Expression<Func<T, bool>> condition) where T : struct, IIdItem
            => _data.Values.OfType<T>().Any(condition.Compile())
            ? _data.Values.OfType<T>().First(condition.Compile())
            : (T?)null;

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public Task<T?> GetFirstStructWhereAsync<T>(Expression<Func<T, bool>> condition) where T : struct, IIdItem
            => Task.FromResult(GetFirstStructWhere(condition));

        /// <summary>
        /// Gets the first item in the data store of the given type which satisfies the given
        /// condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>The first item in the data store of the given type.</returns>
        public async Task<T?> GetFirstStructWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : struct, IIdItem
            => await _data.Values.OfType<T>().ToAsyncEnumerable().AnyAwaitAsync(condition.Compile()).ConfigureAwait(false)
            ? await _data.Values.OfType<T>().ToAsyncEnumerable().FirstAwaitAsync(condition.Compile()).ConfigureAwait(false)
            : (T?)null;

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
        public T? GetFirstStructWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            if (!_data.Values.OfType<T>().Any(x => condition.Compile().Invoke(x)))
            {
                return null;
            }
            return descending
                ? _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderByDescending(selector.Compile()).First()
                : _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderBy(selector.Compile()).First();
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
        public Task<T?> GetFirstStructWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
            => Task.FromResult(GetFirstStructWhereOrderedBy(condition, selector, descending));

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
        public async Task<T?> GetFirstStructWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : struct, IIdItem
        {
            if (!await _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderByDescending(selector.Compile()).AnyAsync().ConfigureAwait(false))
            {
                return null;
            }
            return descending
                ? await _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderByDescending(selector.Compile()).FirstAsync().ConfigureAwait(false)
                : await _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderBy(selector.Compile()).FirstAsync().ConfigureAwait(false);
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
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            else if (_data.TryGetValue(id, out var item))
            {
                return item as T;
            }
            else
            {
                return null;
            }
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
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem => Task.FromResult(GetItem<T>(id));

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItems<T>() where T : IIdItem => _data.Values.OfType<T>().ToList().AsReadOnly();

        /// <summary>
        /// Gets all items in the data store of the given type.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IAsyncEnumerable<T> GetItemsAsync<T>() where T : IIdItem => GetItems<T>().ToAsyncEnumerable();

        /// <summary>
        /// Gets all items in the data store of the given type, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItemsOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
            => descending
            ? _data.Values.OfType<T>().OrderByDescending(selector.Compile()).ToList().AsReadOnly()
            : _data.Values.OfType<T>().OrderBy(selector.Compile()).ToList().AsReadOnly();

        /// <summary>
        /// Gets all items in the data store of the given type, in
        /// the given order.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <typeparam name="TKey">The type of the field used to sort the items.</typeparam>
        /// <param name="selector">A function to select the field by which results will be
        /// ordered.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IAsyncEnumerable<T> GetItemsOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
            => GetItemsOrderedBy(selector, descending).ToAsyncEnumerable();

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IReadOnlyList<T> GetItemsWhere<T>(Expression<Func<T, bool>> condition) where T : IIdItem
            => _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).ToList().AsReadOnly();

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to retrieve.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IAsyncEnumerable<T> GetItemsWhereAsync<T>(Expression<Func<T, bool>> condition) where T : IIdItem
            => GetItemsWhere(condition).ToAsyncEnumerable();

        /// <summary>
        /// Gets all items in the data store of the given type which satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of items to enumerate.</typeparam>
        /// <param name="condition">A condition which items must satisfy.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> of items in the data store of the given
        /// type.</returns>
        public IAsyncEnumerable<T> GetItemsWhereAwaitAsync<T>(Expression<Func<T, ValueTask<bool>>> condition) where T : IIdItem
            => _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile());

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
        public IReadOnlyList<T> GetItemsWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
            => descending
            ? _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderByDescending(selector.Compile()).ToList().AsReadOnly()
            : _data.Values.OfType<T>().Where(x => condition.Compile().Invoke(x)).OrderBy(selector.Compile()).ToList().AsReadOnly();

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
        public IAsyncEnumerable<T> GetItemsWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
            => GetItemsWhereOrderedBy(condition, selector, descending).ToAsyncEnumerable();

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
        public IAsyncEnumerable<T> GetItemsWhereOrderedByAwaitAsync<T, TKey>(Expression<Func<T, ValueTask<bool>>> condition, Expression<Func<T, TKey>> selector, bool descending = false) where T : IIdItem
            => descending
            ? _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderByDescending(selector.Compile())
            : _data.Values.OfType<T>().ToAsyncEnumerable().WhereAwait(condition.Compile()).OrderBy(selector.Compile());

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
        public IPagedList<T> GetPage<T>(int pageNumber, int pageSize) => _data.Values
            .OfType<T>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count());

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
        public Task<IPagedList<T>> GetPageAsync<T>(int pageNumber, int pageSize)
            => Task.FromResult(GetPage<T>(pageNumber, pageSize));

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
        public IPagedList<T> GetPageOrderedBy<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false) => descending
            ? _data.Values
                .OfType<T>()
                .OrderByDescending(selector.Compile())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count())
            : _data.Values
                .OfType<T>()
                .OrderBy(selector.Compile())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count());

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
        public Task<IPagedList<T>> GetPageOrderedByAsync<T, TKey>(Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
            => Task.FromResult(GetPageOrderedBy(selector, pageNumber, pageSize, descending));

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
        public IPagedList<T> GetPageWhere<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize) => _data.Values
            .OfType<T>()
            .Where(condition.Compile())
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count());

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
        public Task<IPagedList<T>> GetPageWhereAsync<T>(Expression<Func<T, bool>> condition, int pageNumber, int pageSize)
            => Task.FromResult(GetPageWhere<T>(condition, pageNumber, pageSize));

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
        public IPagedList<T> GetPageWhereOrderedBy<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false) => descending
            ? _data.Values
                .OfType<T>()
                .Where(condition.Compile())
                .OrderByDescending(selector.Compile())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count())
            : _data.Values
                .OfType<T>()
                .Where(condition.Compile())
                .OrderBy(selector.Compile())
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsPagedList(pageNumber, pageSize, _data.Values.OfType<T>().Count());

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
        public Task<IPagedList<T>> GetPageWhereOrderedByAsync<T, TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> selector, int pageNumber, int pageSize, bool descending = false)
            => Task.FromResult(GetPageWhereOrderedBy(condition, selector, pageNumber, pageSize, descending));

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
        /// <see cref="GetItemsWhere{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public T? GetStruct<T>(string? id) where T : struct, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            else if (_data.TryGetValue(id, out var item) && item is T tItem)
            {
                return tItem;
            }
            else
            {
                return null;
            }
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
        /// <see cref="GetItemsWhereAsync{T}(Expression{Func{T, bool}})"/> with an appropriately formed
        /// condition.
        /// </remarks>
        public Task<T?> GetStructAsync<T>(string? id) where T : struct, IIdItem => Task.FromResult(GetStruct<T>(id));

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
        public bool RemoveItem<T>(string? id) where T : IIdItem => string.IsNullOrEmpty(id) || _data.Remove(id);

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
        public bool RemoveItem(IIdItem? item) => item is null || _data.Remove(item.Id);

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
        public Task<bool> RemoveItemAsync<T>(string? id) where T : IIdItem => Task.FromResult(RemoveItem<T>(id));

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
        public Task<bool> RemoveItemAsync(IIdItem? item) => Task.FromResult(RemoveItem(item));

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
            _data[item.Id] = item;
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
        public Task<bool> StoreItemAsync<T>(T? item) where T : class, IIdItem => Task.FromResult(StoreItem(item));

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
            _data[item.Value.Id] = item.Value;
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
        public Task<bool> StoreStructAsync<T>(T? item) where T : struct, IIdItem => Task.FromResult(StoreStruct(item));
    }
}
