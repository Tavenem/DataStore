using System.Collections.Generic;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// A list of items which is a subset of a larger collection, with information about the place
    /// of this subset within the overall collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public interface IPagedList<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Return the paged query result.
        /// </summary>
        /// <param name="index">Index to fetch item from paged query result.</param>
        /// <returns>Item from paged query result.</returns>
        T this[int index] { get; }

        /// <summary>
        /// The number of records in the paged query result.
        /// </summary>
        long Count { get; }

        /// <summary>
        /// The zero-based index of the first item in the current page, within the whole collection.
        /// </summary>
        long FirstItemOnPage { get; }

        /// <summary>
        /// Whether there is next page available.
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Whether there is a previous page available.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Whether the current page is the first page.
        /// </summary>
        bool IsFirstPage { get; }

        /// <summary>
        /// Whether the current page is the last page.
        /// </summary>
        bool IsLastPage { get; }

        /// <summary>
        /// The zero-based index of the last item in the current page, within the whole collection.
        /// </summary>
        long LastItemOnPage { get; }

        /// <summary>
        /// The current page number.
        /// </summary>
        long PageNumber { get; }

        /// <summary>
        /// The page size.
        /// </summary>
        long PageSize { get; }

        /// <summary>
        /// The number of pages.
        /// </summary>
        long PageCount { get; }

        /// <summary>
        /// The total number of records.
        /// </summary>
        long TotalItemCount { get; }
    }
}
