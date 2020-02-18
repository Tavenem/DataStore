using MartenPagination = Marten.Pagination;

namespace NeverFoundry.DataStorage.Marten
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a <see cref="IPagedList{T}"/> wrapper for the Marten implementation.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="pagedList">The Marten <see cref="MartenPagination.IPagedList{T}"/>.</param>
        /// <returns>An <see cref="IPagedList{T}"/> containing the items in the current collection.</returns>
        public static IPagedList<T> AsPagedList<T>(this MartenPagination.IPagedList<T> pagedList)
            => new MartenPagedListWrapper<T>(pagedList);
    }
}
