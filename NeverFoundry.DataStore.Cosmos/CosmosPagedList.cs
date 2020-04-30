using System.Collections.Generic;

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
        /// <param name="totalItemCount">The total number of records.</param>
        /// <param name="continuationToken">
        /// A continuation token which can be used to resume iteration on the underlying collection.
        /// </param>
        public CosmosPagedList(
            IEnumerable<T>? collection,
            long pageNumber,
            long pageSize,
            long totalItemCount,
            string? continuationToken = null)
            : base(collection, pageNumber, pageSize, totalItemCount)
            => ContinuationToken = continuationToken;
    }
}
