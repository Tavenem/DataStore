using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.Cosmos
{
    /// <summary>
    /// A specialized <see cref="PagedList{T}"/> for use with Azure Cosmos DB which preserves the
    /// continuation token.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class CosmosPagedList<T> : PagedList<T>
    {
        /// <summary>
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </summary>
        public string? ContinuationToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosPagedList{T}"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity to accommodate
        /// the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new
        /// list.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        public CosmosPagedList(
            IEnumerable<T>? collection,
            long pageNumber,
            long pageSize,
            long totalCount,
            string? continuationToken = null)
            : base(collection, pageNumber, pageSize, totalCount)
            => ContinuationToken = continuationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosPagedList{T}"/> class that contains
        /// elements copied from the specified <see cref="FeedIterator{T}"/> and has sufficient
        /// capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="iterator">The <see cref="FeedIterator{T}"/> whose elements are copied to
        /// the new list.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
        public static async Task<CosmosPagedList<T>> FromFeedIteratorAsync(
            FeedIterator<T>? iterator,
            long pageNumber,
            long pageSize,
            long totalCount)
        {
            var collection = new List<T>();
            string? continuationToken = null;
            if (!(iterator is null))
            {
                while (iterator.HasMoreResults)
                {
                    if (collection.Count == pageSize)
                    {
                        break;
                    }
                    var set = await iterator.ReadNextAsync().ConfigureAwait(false);
                    continuationToken = set.ContinuationToken;
                    foreach (var item in set)
                    {
                        if (collection.Count == pageSize)
                        {
                            break;
                        }
                        collection.Add(item);
                    }
                }
            }
            return new CosmosPagedList<T>(collection, pageNumber, pageSize, totalCount, continuationToken);
        }
    }
}
