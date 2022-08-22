namespace Tavenem.DataStorage;

/// <summary>
/// <para>
/// A data transfer object which can be used to reconstitute am <see cref="IPagedList{T}"/>.
/// </para>
/// <para>
/// Suitable for source generation of JSON (de)serialization.
/// </para>
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public class PagedListDTO<T>
{
    /// <summary>
    /// The current page of items.
    /// </summary>
    public IList<T> List { get; set; }

    /// <summary>
    /// <para>
    /// The current page number.
    /// </para>
    /// <para>
    /// The first page is 1.
    /// </para>
    /// </summary>
    public long PageNumber { get; set; } = 1;

    /// <summary>
    /// The page size.
    /// </summary>
    public long PageSize { get; set; }

    /// <summary>
    /// The total number of results, of which this page is a subset.
    /// </summary>
    public long? TotalCount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListDTO{T}"/> class.
    /// </summary>
    public PagedListDTO() => List = new List<T>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListDTO{T}"/> class that contains
    /// elements copied from the specified collection and has sufficient capacity to
    /// accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new
    /// list.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of results, of which this page is a subset.</param>
    public PagedListDTO(IEnumerable<T>? collection, long pageNumber, long pageSize, long? totalCount)
    {
        List = collection?.ToList() ?? new List<T>();
        PageNumber = Math.Max(1, pageNumber);
        PageSize = Math.Max(0, pageSize);
        TotalCount = totalCount;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedListDTO{T}"/> class that contains
    /// the values of the given <see cref="IPagedList{T}"/>.
    /// </summary>
    /// <param name="pagedList">An <see cref="IPagedList{T}"/> to copy.</param>
    public PagedListDTO(IPagedList<T>? pagedList)
    {
        List = pagedList?.ToList() ?? new();
        PageNumber = pagedList?.PageNumber ?? 1;
        PageSize = pagedList?.PageSize ?? 0;
        TotalCount = pagedList?.TotalCount;
    }

    /// <summary>
    /// Returns an <see cref="IPagedList{T}"/> that has the properties of this object.
    /// </summary>
    public IPagedList<T> ToPagedList()
        => new PagedList<T>(List, PageNumber, PageSize, TotalCount);
}
