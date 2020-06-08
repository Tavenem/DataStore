using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.EntityFramework
{
    /// <summary>
    /// <para>
    /// A data store for <see cref="IIdItem"/> instances backed by Entity Framework (Core).
    /// </para>
    /// <para>
    /// Note: although the <see cref="IIdItem"/> interface may apply to classes or structs, the
    /// Entity Framework implementation further restricts most usage to reference types (classes).
    /// </para>
    /// </summary>
    public class EntityFrameworkDataStore : IDataStore
    {
        /// <summary>
        /// The <see cref="DbContext"/> used for all transactions.
        /// </summary>
        public DbContext Context { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="EntityFrameworkDataStore"/>.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> used for all transactions.</param>
        public EntityFrameworkDataStore(DbContext context) => Context = context;

        /// <summary>
        /// Creates a new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IIdItem"/> for which to generate an
        /// id.</typeparam>
        /// <returns>A new <see cref="IIdItem.Id"/> for an <see cref="IIdItem"/> of the given
        /// type.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="EntityFrameworkDataStore"/> implementation generates a new <see cref="Guid"/>
        /// and returns the result of its <see cref="Guid.ToString()"/> method.
        /// </para>
        /// </remarks>
        public string CreateNewIdFor<T>() where T : class, IIdItem => Guid.NewGuid().ToString();

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
        public T? GetItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return Context.Find<T>(id);
        }

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
        public async Task<T?> GetItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return default;
            }
            return await Context.FindAsync<T>(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an <see cref="IDataStoreQueryable{T}"/> of the given type of item.
        /// </summary>
        /// <typeparam name="T">The type of item to query.</typeparam>
        /// <returns>An <see cref="IDataStoreQueryable{T}"/> of the given type of item.</returns>
        public IDataStoreQueryable<T> Query<T>() where T : class, IIdItem
            => new EntityFrameworkDataStoreQueryable<T>(Context.Set<T>().AsQueryable());

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
        public bool RemoveItem<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            var entity = Context.Find<T>(id);
            if (entity is null)
            {
                return true;
            }
            Context.Remove(entity);
            Context.SaveChanges();
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
            Context.Remove(item);
            Context.SaveChanges();
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
        public async Task<bool> RemoveItemAsync<T>(string? id) where T : class, IIdItem
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            var entity = await Context.FindAsync<T>(id).ConfigureAwait(false);
            if (entity is null)
            {
                return true;
            }
            Context.Remove(entity);
            await Context.SaveChangesAsync().ConfigureAwait(false);
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
            Context.Remove(item);
            await Context.SaveChangesAsync().ConfigureAwait(false);
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
            var entity = Context.Find<T>(item.Id);
            if (entity is null)
            {
                Context.Add(item);
            }
            else
            {
                Context.Update(item);
            }
            Context.SaveChanges();
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
            var entity = Context.Find<T>(item.Id);
            if (entity is null)
            {
                await Context.AddAsync(item).ConfigureAwait(false);
            }
            else
            {
                Context.Update(item);
            }
            await Context.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
