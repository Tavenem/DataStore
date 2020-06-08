using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeverFoundry.DataStorage.Cosmos
{
    /// <summary>
    /// Provides LINQ operations on a <see cref="CosmosDataStore"/>, after an ordering operation.
    /// </summary>
    public class OrderedCosmosDataStoreQueryable<T> : CosmosDataStoreQueryable<T>, IOrderedDataStoreQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OrderedCosmosDataStoreQueryable{T}"/>.
        /// </summary>
        public OrderedCosmosDataStoreQueryable(Container container, IOrderedQueryable<T> source) : base(container, source) { }

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
        public IOrderedDataStoreQueryable<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector, bool descending = false)
            => descending
                ? new OrderedCosmosDataStoreQueryable<T>(_container, ((IOrderedQueryable<T>)_source).ThenByDescending(keySelector))
                : new OrderedCosmosDataStoreQueryable<T>(_container, ((IOrderedQueryable<T>)_source).ThenBy(keySelector));
    }
}
