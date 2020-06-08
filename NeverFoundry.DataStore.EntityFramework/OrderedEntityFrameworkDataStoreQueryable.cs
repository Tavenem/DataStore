using System;
using System.Linq;
using System.Linq.Expressions;

namespace NeverFoundry.DataStorage.EntityFramework
{
    /// <summary>
    /// Provides LINQ operations on an <see cref="EntityFrameworkDataStore"/>, after an ordering operation.
    /// </summary>
    public class OrderedEntityFrameworkDataStoreQueryable<T> : EntityFrameworkDataStoreQueryable<T>, IOrderedDataStoreQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OrderedEntityFrameworkDataStoreQueryable{T}"/>.
        /// </summary>
        public OrderedEntityFrameworkDataStoreQueryable(IOrderedQueryable<T> source) : base(source) { }

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
                ? new OrderedEntityFrameworkDataStoreQueryable<T>(((IOrderedQueryable<T>)_source).ThenByDescending(keySelector))
                : new OrderedEntityFrameworkDataStoreQueryable<T>(((IOrderedQueryable<T>)_source).ThenBy(keySelector));
    }
}
