using System.Collections.Generic;

namespace NeverFoundry.DataStorage.Cosmos
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a <see cref="CosmosPagedList{T}"/> wrapper for the current collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The current collection.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalItemCount">The total number of records.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> containing the items in the current collection.</returns>
        public static CosmosPagedList<T> AsCosmosPagedList<T>(
            this IEnumerable<T> collection,
            long pageNumber,
            long pageSize,
            long totalItemCount,
            string? continuationToken = null)
            => new CosmosPagedList<T>(collection, pageNumber, pageSize, totalItemCount, continuationToken);
    }
}
