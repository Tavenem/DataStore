using System.Collections.Generic;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a <see cref="IPagedList{T}"/> wrapper for the current collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The current collection.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalItemCount">The total number of records.</param>
        /// <returns>An <see cref="IPagedList{T}"/> containing the items in the current collection.</returns>
        public static IPagedList<T> AsPagedList<T>(this IEnumerable<T> collection, long pageNumber, long pageSize, long totalItemCount)
            => new PagedList<T>(collection, pageNumber, pageSize, totalItemCount);
    }
}
