using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeverFoundry.DataStorage.EntityFramework
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
        /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
        /// <returns>An <see cref="IPagedList{T}"/> containing the items in the current collection.</returns>
        public static async Task<IPagedList<T>> AsPagedListAsync<T>(this IQueryable<T> collection, long pageNumber, long pageSize, long totalCount)
        {
            var list = new List<T>();
            await foreach (var item in collection.AsAsyncEnumerable())
            {
                list.Add(item);
            }
            return new PagedList<T>(list, pageNumber, pageSize, totalCount);
        }
    }
}
