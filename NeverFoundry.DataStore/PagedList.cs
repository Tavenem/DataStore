using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeverFoundry.DataStorage
{
    /// <summary>
    /// A list of items which is a subset of a larger collection, with information about the place
    /// of this subset within the overall collection.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        private readonly List<T> _list;

        /// <summary>
        /// Return the paged query result.
        /// </summary>
        /// <param name="index">Index to fetch item from paged query result.</param>
        /// <returns>Item from paged query result.</returns>
        public T this[int index] => _list[index];

        /// <summary>
        /// The number of records in the paged query result.
        /// </summary>
        public long Count => _list.Count;

        /// <summary>
        /// The zero-based index of the first item in the current page, within the whole collection.
        /// </summary>
        public long FirstItemOnPage => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Whether there is next page available.
        /// </summary>
        public bool HasNextPage => TotalItemCount > LastItemOnPage + 1;

        /// <summary>
        /// Whether there is a previous page available.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 0;

        /// <summary>
        /// Whether the current page is the first page.
        /// </summary>
        public bool IsFirstPage => PageNumber == 0;

        /// <summary>
        /// Whether the current page is the last page.
        /// </summary>
        public bool IsLastPage => TotalItemCount <= LastItemOnPage + 1;

        /// <summary>
        /// The zero-based index of the last item in the current page, within the whole collection.
        /// </summary>
        public long LastItemOnPage => FirstItemOnPage + (Count - 1);

        /// <summary>
        /// The current page number.
        /// </summary>
        public long PageNumber { get; }

        /// <summary>
        /// The page size.
        /// </summary>
        public long PageSize { get; }

        /// <summary>
        /// The number of pages.
        /// </summary>
        public long PageCount => TotalItemCount % PageSize == 0
            ? TotalItemCount / PageSize
            : (TotalItemCount / PageSize) + 1;

        /// <summary>
        /// The total number of records.
        /// </summary>
        public long TotalItemCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new
        /// list.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="totalItemCount">The total number of records.</param>
        public PagedList(IEnumerable<T>? collection, long pageNumber, long pageSize, long totalItemCount)
        {
            _list = collection?.ToList() ?? new List<T>();
            PageNumber = Math.Max(0, pageNumber);
            PageSize = Math.Max(0, pageSize);
            TotalItemCount = Math.Max(0, totalItemCount);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="PagedList{T}"/>.</returns>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="PagedList{T}"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> for the <see cref="PagedList{T}"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }
}
