using System;
using System.Linq.Expressions;

namespace Tavenem.DataStorage
{
    /// <summary>
    /// Provides LINQ operations on an <see cref="IDataStore"/> implementation's data source, after
    /// an ordering operation.
    /// </summary>
    public interface IOrderedDataStoreQueryable<T> : IDataStoreQueryable<T>
    {
        /// <summary>
        /// Performs a subsequent ordering of the elements in this <see
        /// cref="IOrderedDataStoreQueryable{T}"/> in the given order.
        /// </summary>
        /// <typeparam name="TKey">The type of the key returned by the function represented by
        /// <paramref name="keySelector"/>.</typeparam>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="descending">Whether results will be ordered in descending order.</param>
        /// <returns>
        /// An <see cref="IOrderedDataStoreQueryable{T}"/> whose elements are sorted according to a
        /// key.
        /// </returns>
        IOrderedDataStoreQueryable<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false);
    }
}
