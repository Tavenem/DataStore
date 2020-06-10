using System.Collections;
using System.Collections.Generic;
using MartenPagination = Marten.Pagination;

namespace NeverFoundry.DataStorage.Marten
{
    internal class MartenPagedListWrapper<T> : IPagedList<T>
    {
        private readonly MartenPagination.IPagedList<T> _list;

        /// <summary>
        /// Return the paged query result.
        /// </summary>
        /// <param name="index">Index to fetch item from paged query result.</param>
        /// <returns>Item from paged query result.</returns>
        public T this[int index] => _list[index];

        /// <summary>
        /// The number of records in the paged query result.
        /// </summary>
        public int Count => _list.Count <= int.MaxValue
            ? (int)_list.Count
            : int.MaxValue;

        /// <summary>
        /// The zero-based index of the first item in the current page, within the whole collection.
        /// </summary>
        public long FirstIndexOnPage => _list.FirstItemOnPage - 1;

        /// <summary>
        /// Whether there is next page available.
        /// </summary>
        public bool HasNextPage => _list.HasNextPage;

        /// <summary>
        /// Whether there is a previous page available.
        /// </summary>
        public bool HasPreviousPage => _list.HasPreviousPage;

        /// <summary>
        /// The zero-based index of the last item in the current page, within the whole collection.
        /// </summary>
        public long LastIndexOnPage => _list.LastItemOnPage - 1;

        /// <summary>
        /// The current page number.
        /// </summary>
        public long PageNumber => _list.PageNumber;

        /// <summary>
        /// The page size.
        /// </summary>
        public long PageSize => _list.PageSize;

        /// <summary>
        /// The total number of results, of which this page is a subset.
        /// </summary>
        public long TotalCount => _list.TotalItemCount;

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public long TotalPages => _list.PageCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity to
        /// accommodate the number of elements copied.
        /// </summary>
        /// <param name="list">The collection whose elements are copied to the new
        /// list.</param>
        public MartenPagedListWrapper(MartenPagination.IPagedList<T> list) => _list = list;

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
