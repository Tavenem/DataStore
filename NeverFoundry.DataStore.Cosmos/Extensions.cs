using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> containing the items in the current collection.</returns>
        public static CosmosPagedList<T> AsCosmosPagedList<T>(
            this IEnumerable<T> collection,
            long pageNumber,
            long pageSize,
            long totalCount,
            string? continuationToken = null)
            => new CosmosPagedList<T>(collection, pageNumber, pageSize, totalCount, continuationToken);

        /// <summary>
        /// Returns a <see cref="CosmosPagedList{T}"/> wrapper for the current <see
        /// cref="FeedIterator{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="iterator">The current <see cref="FeedIterator{T}"/>.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
        /// <returns>A <see cref="CosmosPagedList{T}"/> containing the items in the current
        /// collection.</returns>
        public static Task<CosmosPagedList<T>> AsCosmosPagedListAsync<T>(
            this FeedIterator<T>? iterator,
            long pageNumber,
            long pageSize,
            long totalCount)
            => CosmosPagedList<T>.FromFeedIteratorAsync(iterator, pageNumber, pageSize, totalCount);
    }
}
